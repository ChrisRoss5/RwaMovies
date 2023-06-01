﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RwaMovies.DTOs;
using RwaMovies.Exceptions;
using RwaMovies.Models;
using RwaMovies.Services;

namespace RwaMovies.Controllers
{
	public class VideosController : Controller
	{
		private readonly RwaMoviesContext _context;
		private readonly IVideosService _videosService;
		private readonly IGenresService _genresService;
		private readonly ITagsService _tagsService;
		private readonly IImagesService _imagesService;
		private readonly IMapper _mapper;

		public VideosController(RwaMoviesContext context, IVideosService videosService, IGenresService genresService, ITagsService tagsService, IImagesService imagesService, IMapper mapper)
		{
			_context = context;
			_videosService = videosService;
			_genresService = genresService;
			_tagsService = tagsService;
			_imagesService = imagesService;
			_mapper = mapper;
		}

		public async Task<IActionResult> Index(string? nameFilter, string? genreFilter, int? pageNum)
		{
			try
			{
				return View(new VideosVM
				{
					SearchResult = await _videosService.Search(new SearchParams
					{
						NameFilter = nameFilter,
						GenreFilter = genreFilter,
						PageNum = pageNum
					}),
					Genres = new SelectList((await _genresService.GetGenres()).Select(g => g.Name)),
					NameFilter = nameFilter,
					GenreFilter = genreFilter,
				});
			}
			catch (Exception ex)
			{
				if (ex is BadRequestException)
					return BadRequest();
				return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error!");
			}
		}

		public async Task<IActionResult> IndexPartial(string? nameFilter, string? genreFilter, int? pageNum)
		{
			try
			{
				var searchResult = await _videosService.Search(new SearchParams
				{
					NameFilter = nameFilter,
					GenreFilter = genreFilter,
					PageNum = pageNum
				});
				return PartialView("_VideoList", searchResult.Videos);
			}
			catch (Exception ex)
			{
				if (ex is BadRequestException)
					return BadRequest();
				return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error!");
			}
		}

		public async Task<IActionResult> Details(int id)
		{
			try
			{
				return View(await _videosService.GetVideo(id));
			}
			catch
			{
				return NotFound();
			}
		}

		public async Task<IActionResult> Create()
		{
			await PopulateVideoViewBag();
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(VideoFormVM videoFormVM)
		{
			if (!ModelState.IsValid)
			{
				await PopulateVideoViewBag();
				return View(videoFormVM);
			}
			try
			{
				if (videoFormVM.ImageFile != null)
				{
					videoFormVM.VideoRequest.ImageId =
						await _imagesService.PostImage(videoFormVM.ImageFile);
				}
				int videoId = await _videosService.PostVideo(videoFormVM.VideoRequest);
				return RedirectToAction(nameof(Details), new { id = videoId });
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error!");
			}
		}

		public async Task<IActionResult> Edit(int id)
		{
			try
			{
				var videoResponse = await _videosService.GetVideo(id);
				await PopulateVideoViewBag();
				return View(new VideoFormVM
				{
					VideoRequest = _mapper.Map<VideoRequest>(videoResponse),
				});
			}
			catch
			{
				return NotFound();
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, VideoFormVM videoFormVM, string? imageOption)
		{
			if (id != videoFormVM.VideoRequest.Id)
				return NotFound();
			if (!ModelState.IsValid)
			{
				await PopulateVideoViewBag();
				return View(videoFormVM);
			}
			try
			{
				if (imageOption == "delete")
				{
					await _imagesService.DeleteImage((int)videoFormVM.VideoRequest.ImageId!);
					videoFormVM.VideoRequest.ImageId = null;
					_context.ChangeTracker.Clear();
                }
				else if (videoFormVM.ImageFile != null)
					if (videoFormVM.VideoRequest.ImageId != null)
						await _imagesService.PutImage(
							(int)videoFormVM.VideoRequest.ImageId, videoFormVM.ImageFile);
					else
						videoFormVM.VideoRequest.ImageId =
							await _imagesService.PostImage(videoFormVM.ImageFile);
				await _videosService.PutVideo(id, videoFormVM.VideoRequest);
				return RedirectToAction(nameof(Details), new { id });
			}
			catch (Exception ex)
			{
				if (ex is NotFoundException)
					return NotFound();
				return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error!");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				await _videosService.DeleteVideo(id);
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				if (ex is NotFoundException)
					return NotFound();
				return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error!");
			}
		}
		private async Task PopulateVideoViewBag()
		{
			var genres = await _genresService.GetGenres();
			var tags = await _tagsService.GetTags();
			ViewBag.Genres = new SelectList(genres, "Id", "Name");
			ViewBag.Tags = new SelectList(tags, "Id", "Name");
		}
	}
}
