﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RwaMovies.Exceptions;
using RwaMovies.Services;
using RwaMovies.Models.DAL;
using RwaMovies.Models.Shared;

namespace RwaMovies.Controllers.Views
{
    public class TagsController : Controller
    {
        private readonly RwaMoviesContext _context;
        private readonly ITagsService _tagsService;

        public TagsController(RwaMoviesContext context, ITagsService tagsService)
        {
            _context = context;
            _tagsService = tagsService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _tagsService.GetTags());
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                return View(await _tagsService.GetTag(id));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TagDTO tagDTO)
        {
            if (!ModelState.IsValid)
                return View(tagDTO);
            await _tagsService.PostTag(tagDTO);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                return View(await _tagsService.GetTag(id));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TagDTO tagDTO)
        {
            if (id != tagDTO.Id)
                return NotFound();
            if (!ModelState.IsValid)
                return View(tagDTO);
            try
            {
                await _tagsService.PutTag(id, tagDTO);
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                return View(await _tagsService.GetTag(id));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _tagsService.DeleteTag(id);
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
    }
}
