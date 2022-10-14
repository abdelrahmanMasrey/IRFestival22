using System.Net;

using Microsoft.AspNetCore.Mvc;

using IRFestival.Api.Data;
using IRFestival.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.ApplicationInsights;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FestivalController : ControllerBase
    {
        public FestivalDbContext db { get; set; }

        public TelemetryClient Tc { get; set; }

        public FestivalController(FestivalDbContext dbContext, TelemetryClient tc)
        {
            db = dbContext;
            Tc = tc;
        }
        [HttpGet("LineUp")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Schedule))]
        public ActionResult GetLineUp()
        {
            return Ok(FestivalDataSource.Current.LineUp);
        }

        [HttpGet("Artists")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Artist>))]
        public ActionResult GetArtists(bool? withRatings)
        {
            if (withRatings.HasValue && withRatings.Value)
                Tc.TrackEvent($"list of ar with rat");
            else
                Tc.TrackEvent($"list of ar without rat");

           /* var artists = await db.Artists.ToListAsync();*/
            return base.Ok(FestivalDataSource.Current.Artists);
        }

        [HttpGet("Stages")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Stage>))]
        public async Task<ActionResult> GetStages()
        {
            var artists = await db.Stages.ToListAsync();
            return Ok(artists);
        }

        [HttpPost("Favorite")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ScheduleItem))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult SetAsFavorite(int id)
        {
            var schedule = FestivalDataSource.Current.LineUp.Items
                .FirstOrDefault(si => si.Id == id);
            if (schedule != null)
            {
                schedule.IsFavorite = !schedule.IsFavorite;
                return Ok(schedule);
            }
            return NotFound();
        }

    }
}