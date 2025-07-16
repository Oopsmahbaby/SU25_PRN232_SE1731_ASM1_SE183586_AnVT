using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SmokeQuit.Repositories.AnVT.ModelExtensions;
using SmokeQuit.Repositories.AnVT.Models;
using System.Text;
using X.PagedList;

namespace SmokeQuit.MVCWebApp.FE.AnVT.Controllers
{
	public class BlogPostsAnVtsController : Controller
	{
		private string APIEndPoint = "https://localhost:7184/api/";

		public IActionResult QuitPlansDuongLnt()
		{
			return View();
		}

		// GET: BlogPostsAnVts
		public async Task<IActionResult> Index(string? tille, string? category, string? tag, int pageNumber = 1)
		{
			BlogPostSearchRequest searchRequestWithFilter = new BlogPostSearchRequest
			{
				Title = tille,
				Category = category,
				Tags = tag,
				PageNumber = pageNumber,
				PageSize = 3 // Số lượng bản ghi trên mỗi trang
			};

			using (var httpClient = new HttpClient())
			{
				var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);

				using (var response = await httpClient.PostAsJsonAsync(APIEndPoint + "BlogPostsAnVts/searchWithPaging", searchRequestWithFilter))
				{
					if (response.IsSuccessStatusCode)
					{
						var content = await response.Content.ReadAsStringAsync();

						// Log để kiểm tra dữ liệu nhận được
						Console.WriteLine("API content: " + content);

						var result = JsonConvert.DeserializeObject<PagedResult<BlogPostsAnVt>>(content);

						if (result != null && result.PageNumber > 0 && result.PageSize > 0)
						{
							var pagedList = new StaticPagedList<BlogPostsAnVt>(
								result.Items,
								result.PageNumber,
								result.PageSize,
								result.TotalCount
							);

							return View(pagedList);
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

		// POST: quitPlansAnhDtns/Create
		#region Create
		// GET: blogPostsAnVts/Create
		public async Task<IActionResult> Create()
		{
			ViewBag.PlanList = new SelectList(await GetPlans(), "QuitPlansAnhDtnid", "Reason");

			ViewBag.UserList = new SelectList(await GetUsers(), "UserAccountId", "UserName");
			return View();
		}
		#endregion

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(BlogPostsAnVt blogPostsAnVt)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					TempData["Error"] = "Model state is invalid.";
					//await ReloadDropdowns(blogPostsAnVt);
					//return View(blogPostsAnVt);
				}

				var token = HttpContext.Request.Cookies["TokenString"];

				if (string.IsNullOrEmpty(token))
				{
					TempData["Error"] = "Token is missing.";
					//await ReloadDropdowns(blogPostsAnVt);
					//return View(blogPostsAnVt);
				}

				using var client = new HttpClient();
				client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
				var json = JsonConvert.SerializeObject(blogPostsAnVt);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = await client.PostAsync(APIEndPoint + "BlogPostsAnVts", content);

				if (response.IsSuccessStatusCode)
				{
					TempData["Success"] = "New BlogPostsAnVt is created successfully.";
					return RedirectToAction(nameof(Index));
				}

				var responseBody = await response.Content.ReadAsStringAsync();
				TempData["Error"] = $"Failed to create. Status: {response.StatusCode}. Response: {responseBody}";
			}
			catch (Exception ex)
			{
				TempData["Error"] = "Exception while creating: " + ex.Message;
			}

			//await ReloadDropdowns(blogPostsAnVt);
			return View(blogPostsAnVt);
		}

		// Helper to load dropdowns
		//private async Task ReloadDropdowns(BlogPostsAnVt selected)
		//{
		//	var token = HttpContext.Request.Cookies["TokenString"];
		//	List<QuitPlansAnhDtn> achievements = new();
		//	List<SystemUserAccount> users = new();

		//	if (!string.IsNullOrEmpty(token))
		//	{
		//		try
		//		{
		//			using var client = new HttpClient();
		//			client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

		//			var blogPosts = await client.GetAsync(APIEndPoint + "QuitPlansAnhDtn");
		//			var userRes = await client.GetAsync(APIEndPoint + "SystemUserAccounts/All");

		//			if (blogPosts.IsSuccessStatusCode)
		//				achievements = JsonConvert.DeserializeObject<List<QuitPlansAnhDtn>>(await blogPosts.Content.ReadAsStringAsync());

		//			if (userRes.IsSuccessStatusCode)
		//				users = JsonConvert.DeserializeObject<List<SystemUserAccount>>(await userRes.Content.ReadAsStringAsync());
		//		}
		//		catch (Exception ex)
		//		{
		//			TempData["Error"] += " | Reload dropdowns failed: " + ex.Message;
		//		}
		//	}

		//	await LoadDropdownData(achievements, users, token, selected);
		//}

		// Bind data to dropdowns
		//private Task LoadDropdownData(List<QuitPlansAnhDtn> quitPlanAnhDtn, List<SystemUserAccount> users, string token, BlogPostsAnVt selected = null)
		//{
		//	quitPlanAnhDtn ??= new();
		//	users ??= new();

		//	ViewBag.PlanList = new SelectList(quitPlanAnhDtn, "QuitPlansAnhDtnid", "UserId", selected?.PlanId);
		//	ViewBag.UserList = new SelectList(users, "UserAccountId", "UserName", selected?.UserId);

		//	return Task.CompletedTask;
		//}


		#region Get Plans trong bảng phụ
		private async Task<List<QuitPlansAnhDtn>> GetPlans()
		{
			using (var httpClient = new HttpClient())
			{
				// Add Token to header of Request
				var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;

				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);

				using (var response = await httpClient.GetAsync(APIEndPoint + "QuitPlansAnhDtn_AnVT"))
				{
					if (response.IsSuccessStatusCode)
					{
						var content = await response.Content.ReadAsStringAsync();
						var result = JsonConvert.DeserializeObject<List<QuitPlansAnhDtn>>(content);

						if (result != null)
						{
							return result;
						}
					}
				}
			}

			return new List<QuitPlansAnhDtn>();
		}
		#endregion

		#region Get trong bảng User
		private async Task<List<SystemUserAccount>> GetUsers()
		{
			using (var httpClient = new HttpClient())
			{
				// Add Token to header of Request
				var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;

				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);

				using (var response = await httpClient.GetAsync(APIEndPoint + "SystemUserAccounts"))
				{
					if (response.IsSuccessStatusCode)
					{
						var content = await response.Content.ReadAsStringAsync();
						var result = JsonConvert.DeserializeObject<List<SystemUserAccount>>(content);

						if (result != null)
						{
							return result;
						}
					}
				}
			}

			return new List<SystemUserAccount>();
		}
		#endregion


		// GET: BlogPostsAnVts/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			BlogPostsAnVt blogPostsAnVt = null;

			using (var httpClient = new HttpClient())
			{
				var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);

				var response = await httpClient.GetAsync(APIEndPoint + "BlogPostsAnVts/" + id);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					blogPostsAnVt = JsonConvert.DeserializeObject<BlogPostsAnVt>(content);
				}
			}

			if (blogPostsAnVt == null)
			{
				return NotFound();
			}

			ViewData["UserId"] = new SelectList(await GetUsers(), "UserAccountId", "UserName", blogPostsAnVt.UserId);
			ViewData["PlanId"] = new SelectList(await GetPlans(), "QuitPlansAnhDtnid", "Reason", blogPostsAnVt.PlanId);

			return View(blogPostsAnVt);
		}

		// POST: BlogPostsAnVts/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, BlogPostsAnVt blogPostsAnVt)
		{
			if (id != blogPostsAnVt.BlogPostsAnVtid)
			{
				return NotFound();
			}

			blogPostsAnVt.UserId = 1;
			if (ModelState.IsValid)
			{
				using (var httpClient = new HttpClient())
				{
					var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;
					httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);

					using (var response = await httpClient.PutAsJsonAsync(APIEndPoint + "BlogPostsAnVts/" + id, blogPostsAnVt))
					{
						if (response.IsSuccessStatusCode)
						{
							var content = await response.Content.ReadAsStringAsync();
							var result = JsonConvert.DeserializeObject<int>(content);

							if (result > 0)
							{
								return RedirectToAction(nameof(Index));
							}
						}
					}
				}
			}

			ViewData["UserId"] = new SelectList(await GetUsers(), "UserAccountId", "UserName", blogPostsAnVt.UserId);
			ViewData["PlanId"] = new SelectList(await GetPlans(), "QuitPlansAnhDtnid", "Reason", blogPostsAnVt.PlanId);

			return View(blogPostsAnVt);
		}

		// GET: BlogPostsAnVts/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null) return NotFound();

			if (ModelState.IsValid)
			{
				using (var httpClient = new HttpClient())
				{
					var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;
					httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);

					using (var response = await httpClient.GetAsync(APIEndPoint + "BlogPostsAnVts/" + id))
					{
						if (response.IsSuccessStatusCode)
						{
							var content = await response.Content.ReadAsStringAsync();
							var result = JsonConvert.DeserializeObject<BlogPostsAnVt>(content);

							if (result != null)
							{
								return View(result);
							}
						}
					}
				}
			}
			return View(Index(category: null, tille: null, tag: null));
		}

		// POST: BlogPostsAnVts/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			using (var httpClient = new HttpClient())
			{
				var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;

				if (string.IsNullOrEmpty(tokenString))
				{
					return Unauthorized();
				}

				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);

				var response = await httpClient.DeleteAsync(APIEndPoint + $"BlogPostsAnVts/{id}");

				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					TempData["Success"] = $"BlogPostsAnVt ({id}) is deleted successfully.";
					var deleteResult = JsonConvert.DeserializeObject<bool>(content);

					if (deleteResult)
					{
						return RedirectToAction(nameof(Index));
					}
				}
				else
				{
					var content = await response.Content.ReadAsStringAsync();
					TempData["Error"] = $"Failed to delete. Status: {response.StatusCode}. Response: {content}";
				}
			}

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (id == null) return NotFound();

			using (var httpClient = new HttpClient())
			{
				var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;
				if (string.IsNullOrEmpty(tokenString))
					return Unauthorized();

				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);

				var response = await httpClient.GetAsync(APIEndPoint + $"BlogPostsAnVts/{id}");

				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					var result = JsonConvert.DeserializeObject<BlogPostsAnVt>(content);

					if (result != null)
						return View(result);
				}
			}

			return NotFound();
		}

	}
}
