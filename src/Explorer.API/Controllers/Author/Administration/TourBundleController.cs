﻿using Explorer.API.Services;
using Explorer.Blog.Core.Domain.BlogPosts;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.UseCases.Administration;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author.Administration
{
	[Authorize(Policy = "authorPolicy")]
	[Route("api/administration/tour-bundle")]
	public class TourBundleController : BaseApiController
	{
		private readonly ITourBundleService _tourBundleService;

		public TourBundleController(ITourBundleService tourBundleService) 
		{
			_tourBundleService = tourBundleService;
		}

		[HttpGet]
		public ActionResult<PagedResult<CheckpointDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
		{
			var result = _tourBundleService.GetPaged(page, pageSize);
			return CreateResponse(result);
		}

		[HttpGet("by-author/{id:int}")]
		public ActionResult<List<TourBundleDto>> GetAllByAuthor([FromQuery] int page, [FromQuery] int pageSize, int id)
		{
			var result = _tourBundleService.GetAllByAuthor(page, pageSize, id);
			return CreateResponse(result);
		}

		[HttpPost]
		public ActionResult<TourBundleDto> Create([FromBody] TourBundleDto tourBundle)
		{
			if (User.PersonId() != tourBundle.AuthorId) return CreateResponse(Result.Fail(FailureCode.Forbidden));
			var result = _tourBundleService.Create(tourBundle);
			return CreateResponse(result);
		}

		[HttpPut("{id:int}")]
		public ActionResult<TourBundleDto> Update([FromBody] TourBundleDto tourBundle)
		{
			if (User.PersonId() != tourBundle.AuthorId) return CreateResponse(Result.Fail(FailureCode.Forbidden));
			var result = _tourBundleService.Update(tourBundle);
			return CreateResponse(result);
		}

		[HttpDelete("{id:int}")]
		public ActionResult Delete(int id)
		{
			var result = _tourBundleService.Delete(id);
			return CreateResponse(result);
		}

		[HttpGet("{id:int}")]
		public ActionResult<TourBundleDto> GetById(int id)
		{
			var result = _tourBundleService.GetBundleById(id);
			return CreateResponse(result);
		}

		[HttpPut("remove-tour/{bundleId:int}/{tourId:int}")]
		public ActionResult<TourBundleDto> RemoveTourFromBundle(int bundleId, int tourId)
		{
			var result = _tourBundleService.RemoveTourFromBundle(bundleId, tourId);
			return CreateResponse(result);
		}

		[HttpPut("add-tour/{bundleId:int}/{tourId:int}")]
		public ActionResult<TourBundleDto> AddTourToBundle(int bundleId, int tourId)
		{
			var result = _tourBundleService.AddTourToBundle(bundleId, tourId);
			return CreateResponse(result);
		}
	}
}
