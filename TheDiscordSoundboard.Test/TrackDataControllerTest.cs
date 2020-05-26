using Moq;
using System.Collections.Generic;
using TheDiscordSoundboard.Controllers;
using TheDiscordSoundboard.Models;
using TheDiscordSoundboard.Service;
using Xunit;

namespace TheDiscordSoundboard.Test
{
    public class TrackDataControllerTest
    {

        private IEnumerable<TrackData> GetFakeData()
        {
            return null;   
        }

        private Mock<ITrackDataService> GetContextMock()
        {
            var mock =  new Mock<ITrackDataService>();

            var list = GetFakeData();

            //mock.Setup(x => x.FindById(1)).ReturnsAsync(list.First());

            return mock;
        }


        [Fact]
        public void Get_All_Tracks_Empty()
        {
            var context = GetContextMock();
            var controller = new TrackDataController(context.Object);

        }

        [Fact]
        public void Get_All_Tracks_Filled()
        {
            var context = GetContextMock();
            var controller = new TrackDataController(context.Object);
        }



    }
}
