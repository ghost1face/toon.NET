using Xunit;

namespace Toon.Test
{
    public class ToonSerializerSettingsTests
    {
        [Fact]
        public void ToonSerializerSettings_AssignsValues()
        {
            var settings = new ToonSerializerSettings
            {
                Indent = 2,
                Delimiter = ',',
                LengthMarker = '#'
            };

            Assert.Equal(2, settings.Indent);
            Assert.Equal(',', settings.Delimiter);
            Assert.Equal('#', settings.LengthMarker);
        }
    }
}
