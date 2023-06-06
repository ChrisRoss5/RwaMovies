﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RwaMovies.DTOs;
using RwaMovies.Exceptions;
using RwaMovies.Models;
using RwaMovies.Services;
using RwaMovies.Extensions;

namespace RwaMovies.Controllers
{
    public class GenresController : Controller
    {
        private readonly RwaMoviesContext _context;
        private readonly IGenresService _genresService;

        public GenresController(RwaMoviesContext context, IGenresService genresService)
        {
            _context = context;
            _genresService = genresService;
        }

        public async Task<IActionResult> Index()
        {
            var genres = await _genresService.GetGenres();
            if (Request.IsAjaxRequest()) 
                return PartialView(genres);
            return View(genres);
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var genre = await _genresService.GetGenre(id);
                if (Request.IsAjaxRequest())
					return PartialView(genre);
                return View(genre);
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    return NotFound();
                throw;
            }
        }

        public IActionResult Create()
        {
            if (Request.IsAjaxRequest())
                return PartialView();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GenreDTO genreDTO)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return PartialView(genreDTO);
                return View(genreDTO);
            }
            await _genresService.PostGenre(genreDTO);
            if (Request.IsAjaxRequest())
                return Ok("Success");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var genre = await _genresService.GetGenre(id);
                if (Request.IsAjaxRequest())
                    return PartialView(genre);
                return View(genre);
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    return NotFound();
                throw;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GenreDTO genreDTO)
        {
            if (id != genreDTO.Id)
                return NotFound();
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return PartialView(genreDTO);
                return View(genreDTO);
            }
            try
            {
                await _genresService.PutGenre(id, genreDTO);
                if (Request.IsAjaxRequest())
                    return Ok("Success");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    return NotFound();
                throw;
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var genre = await _genresService.GetGenre(id);
                if (Request.IsAjaxRequest())
                    return PartialView(genre);
                return View(genre);
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    return NotFound();
                throw;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _genresService.DeleteGenre(id);
                if (Request.IsAjaxRequest())
                    return Ok("Success");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException sqlEx && sqlEx.Message.Contains("FK_Video_Genre"))
                {
                    ModelState.AddModelError("", "Cannot delete genre because it is used in a video.");
                    var genre = await _genresService.GetGenre(id);
                    if (Request.IsAjaxRequest())
                        return PartialView(genre);
                    return View(genre);
                }
                if (ex is NotFoundException)
                    return NotFound();
                throw;
            }
        }
    }
}