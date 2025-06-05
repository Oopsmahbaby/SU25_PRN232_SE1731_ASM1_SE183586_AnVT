using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SmokeQuit.Repositories.AnVT.DBContext;
using SmokeQuit.Repositories.AnVT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokeQuit.MVCWebApp.FE.AnVT.Controllers
{
	public class BlogPostsAnVtsController : Controller
	{
		//private readonly SU25_PRN232_SE1731_G6_SmokeQuitContext _context;

		//public BlogPostsAnVtsController(SU25_PRN232_SE1731_G6_SmokeQuitContext context)
		//{
		//    _context = context;
		//}

		private string APIEndPoint = "https://localhost:7184/api/";

		// GET: BlogPostsAnVts
		public async Task<IActionResult> Index()
		{
			using (var httpClient = new HttpClient())
			{
				var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);

				using (var response = await httpClient.GetAsync(APIEndPoint + "BlogPostsAnVts"))
				{
					if (response.IsSuccessStatusCode)
					{
						var content = await response.Content.ReadAsStringAsync();

						// Log để kiểm tra dữ liệu nhận được
						Console.WriteLine("API content: " + content);

						var result = JsonConvert.DeserializeObject<List<BlogPostsAnVt>>(content);

						if (result != null && result.Any())
						{
							return View(result);
						}
					}
					else
					{
						Console.WriteLine("API failed: " + response.StatusCode);
					}
				}
			}

			return View(new List<BlogPostsAnVt>());
		}


		// GET: quitPlansAnhDtns/Create
		public async Task<IActionResult> Create()
		{
			try
			{
				var token = HttpContext.Request.Cookies["TokenString"];
				if (string.IsNullOrEmpty(token))
				{
					TempData["Error"] = "Token is missing.";
					await LoadDropdownData(null, null, null);
					return View(new BlogPostsAnVt());
				}

				List<QuitPlansAnhDtn> achievements = null;
				List<SystemUserAccount> users = null;

				using var client = new HttpClient();
				client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

				var blogPosts = await client.GetAsync(APIEndPoint + "BlogPostsAnVts");
				var userRes = await client.GetAsync(APIEndPoint + "SystemUserAccounts/All"); // <-- đúng endpoint

				if (!blogPosts.IsSuccessStatusCode || !userRes.IsSuccessStatusCode)
				{
					TempData["Error"] = $"Failed to load dropdowns. Achievements: {blogPosts.StatusCode}, Users: {userRes.StatusCode}";
				}

				try
				{
					achievements = JsonConvert.DeserializeObject<List<QuitPlansAnhDtn>>(await blogPosts.Content.ReadAsStringAsync());
				}
				catch (Exception ex)
				{
					TempData["Error"] += $" | Error deserializing achievements: {ex.Message}";
				}

				try
				{
					users = JsonConvert.DeserializeObject<List<SystemUserAccount>>(await userRes.Content.ReadAsStringAsync());
				}
				catch (Exception ex)
				{
					TempData["Error"] += $" | Error deserializing users: {ex.Message}";
				}

				await LoadDropdownData(achievements, users, token);

				return View(new BlogPostsAnVt());
			}
			catch (Exception ex)
			{
				TempData["Error"] = "Unexpected error: " + ex.Message;
				await LoadDropdownData(null, null, null);
				return View(new BlogPostsAnVt());
			}
		}

		// POST: quitPlansAnhDtns/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(BlogPostsAnVt blogPostsAnVt)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					TempData["Error"] = "Model state is invalid.";
					await ReloadDropdowns(blogPostsAnVt);
					return View(blogPostsAnVt);
				}

				var token = HttpContext.Request.Cookies["TokenString"];
				if (string.IsNullOrEmpty(token))
				{
					TempData["Error"] = "Token is missing.";
					await ReloadDropdowns(blogPostsAnVt);
					return View(blogPostsAnVt);
				}

				using var client = new HttpClient();
				client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
				var json = JsonConvert.SerializeObject(blogPostsAnVt);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = await client.PostAsync(APIEndPoint + "BlogPostsAnVts", content);

				if (response.IsSuccessStatusCode)
				{
					TempData["Success"] = "UserAchievement created successfully.";
					return RedirectToAction(nameof(Index));
				}

				var responseBody = await response.Content.ReadAsStringAsync();
				TempData["Error"] = $"Failed to create. Status: {response.StatusCode}. Response: {responseBody}";
			}
			catch (Exception ex)
			{
				TempData["Error"] = "Exception while creating: " + ex.Message;
			}

			await ReloadDropdowns(blogPostsAnVt);
			return View(blogPostsAnVt);
		}

		// Helper to load dropdowns
		private async Task ReloadDropdowns(BlogPostsAnVt selected)
		{
			var token = HttpContext.Request.Cookies["TokenString"];
			List<QuitPlansAnhDtn> achievements = new();
			List<SystemUserAccount> users = new();

			if (!string.IsNullOrEmpty(token))
			{
				try
				{
					using var client = new HttpClient();
					client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

					var blogPosts = await client.GetAsync(APIEndPoint + "QuitPlansAnhDtn");
					var userRes = await client.GetAsync(APIEndPoint + "SystemUserAccounts/All");

					if (blogPosts.IsSuccessStatusCode)
						achievements = JsonConvert.DeserializeObject<List<QuitPlansAnhDtn>>(await blogPosts.Content.ReadAsStringAsync());

					if (userRes.IsSuccessStatusCode)
						users = JsonConvert.DeserializeObject<List<SystemUserAccount>>(await userRes.Content.ReadAsStringAsync());
				}
				catch (Exception ex)
				{
					TempData["Error"] += " | Reload dropdowns failed: " + ex.Message;
				}
			}

			await LoadDropdownData(achievements, users, token, selected);
		}

		// Bind data to dropdowns
		private Task LoadDropdownData(List<QuitPlansAnhDtn> quitPlanAnhDtn, List<SystemUserAccount> users, string token, BlogPostsAnVt selected = null)
		{
			quitPlanAnhDtn ??= new();
			users ??= new();

			ViewBag.PlanList = new SelectList(quitPlanAnhDtn, "QuitPlansAnhDtnid", "UserId", selected?.PlanId);
			ViewBag.UserList = new SelectList(users, "UserAccountId", "UserName", selected?.UserId);

			return Task.CompletedTask;
		}

		// GET: BlogPostsAnVts/Details/5
		//public async Task<IActionResult> Details(int? id)
		//{
		//    if (id == null)
		//    {
		//        return NotFound();
		//    }

		//    var blogPostsAnVt = await _context.BlogPostsAnVts
		//        .Include(b => b.Plan)
		//        .Include(b => b.User)
		//        .FirstOrDefaultAsync(m => m.BlogPostsAnVtid == id);
		//    if (blogPostsAnVt == null)
		//    {
		//        return NotFound();
		//    }

		//    return View(blogPostsAnVt);
		//}

		//// GET: BlogPostsAnVts/Create
		//public IActionResult Create()
		//{
		//    ViewData["PlanId"] = new SelectList(_context.QuitPlansAnhDtns, "QuitPlansAnhDtnid", "Reason");
		//    ViewData["UserId"] = new SelectList(_context.SystemUserAccounts, "UserAccountId", "Email");
		//    return View();
		//}

		//// POST: BlogPostsAnVts/Create
		//// To protect from overposting attacks, enable the specific properties you want to bind to.
		//// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> Create([Bind("BlogPostsAnVtid,UserId,PlanId,Title,Content,Category,Tags,IsPublic,ViewsCount,LikesCount,CommentsCount,CreatedAt,UpdatedAt")] BlogPostsAnVt blogPostsAnVt)
		//{
		//    if (ModelState.IsValid)
		//    {
		//        _context.Add(blogPostsAnVt);
		//        await _context.SaveChangesAsync();
		//        return RedirectToAction(nameof(Index));
		//    }
		//    ViewData["PlanId"] = new SelectList(_context.QuitPlansAnhDtns, "QuitPlansAnhDtnid", "Reason", blogPostsAnVt.PlanId);
		//    ViewData["UserId"] = new SelectList(_context.SystemUserAccounts, "UserAccountId", "Email", blogPostsAnVt.UserId);
		//    return View(blogPostsAnVt);
		//}

		//// GET: BlogPostsAnVts/Edit/5
		//public async Task<IActionResult> Edit(int? id)
		//{
		//    if (id == null)
		//    {
		//        return NotFound();
		//    }

		//    var blogPostsAnVt = await _context.BlogPostsAnVts.FindAsync(id);
		//    if (blogPostsAnVt == null)
		//    {
		//        return NotFound();
		//    }
		//    ViewData["PlanId"] = new SelectList(_context.QuitPlansAnhDtns, "QuitPlansAnhDtnid", "Reason", blogPostsAnVt.PlanId);
		//    ViewData["UserId"] = new SelectList(_context.SystemUserAccounts, "UserAccountId", "Email", blogPostsAnVt.UserId);
		//    return View(blogPostsAnVt);
		//}

		//// POST: BlogPostsAnVts/Edit/5
		//// To protect from overposting attacks, enable the specific properties you want to bind to.
		//// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> Edit(int id, [Bind("BlogPostsAnVtid,UserId,PlanId,Title,Content,Category,Tags,IsPublic,ViewsCount,LikesCount,CommentsCount,CreatedAt,UpdatedAt")] BlogPostsAnVt blogPostsAnVt)
		//{
		//    if (id != blogPostsAnVt.BlogPostsAnVtid)
		//    {
		//        return NotFound();
		//    }

		//    if (ModelState.IsValid)
		//    {
		//        try
		//        {
		//            _context.Update(blogPostsAnVt);
		//            await _context.SaveChangesAsync();
		//        }
		//        catch (DbUpdateConcurrencyException)
		//        {
		//            if (!BlogPostsAnVtExists(blogPostsAnVt.BlogPostsAnVtid))
		//            {
		//                return NotFound();
		//            }
		//            else
		//            {
		//                throw;
		//            }
		//        }
		//        return RedirectToAction(nameof(Index));
		//    }
		//    ViewData["PlanId"] = new SelectList(_context.QuitPlansAnhDtns, "QuitPlansAnhDtnid", "Reason", blogPostsAnVt.PlanId);
		//    ViewData["UserId"] = new SelectList(_context.SystemUserAccounts, "UserAccountId", "Email", blogPostsAnVt.UserId);
		//    return View(blogPostsAnVt);
		//}

		//// GET: BlogPostsAnVts/Delete/5
		//public async Task<IActionResult> Delete(int? id)
		//{
		//    if (id == null)
		//    {
		//        return NotFound();
		//    }

		//    var blogPostsAnVt = await _context.BlogPostsAnVts
		//        .Include(b => b.Plan)
		//        .Include(b => b.User)
		//        .FirstOrDefaultAsync(m => m.BlogPostsAnVtid == id);
		//    if (blogPostsAnVt == null)
		//    {
		//        return NotFound();
		//    }

		//    return View(blogPostsAnVt);
		//}

		//// POST: BlogPostsAnVts/Delete/5
		//[HttpPost, ActionName("Delete")]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> DeleteConfirmed(int id)
		//{
		//    var blogPostsAnVt = await _context.BlogPostsAnVts.FindAsync(id);
		//    if (blogPostsAnVt != null)
		//    {
		//        _context.BlogPostsAnVts.Remove(blogPostsAnVt);
		//    }

		//    await _context.SaveChangesAsync();
		//    return RedirectToAction(nameof(Index));
		//}

		//private bool BlogPostsAnVtExists(int id)
		//{
		//    return _context.BlogPostsAnVts.Any(e => e.BlogPostsAnVtid == id);
		//}
	}
}
