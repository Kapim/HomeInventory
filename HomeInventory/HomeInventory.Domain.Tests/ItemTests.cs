using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Domain.Tests
{
    public class ItemTests
    {
        [Fact]
        public void Ctor_Should_SetName()
        {
            var name = "Flashlight";
            var item = new Item(name);
            Assert.Equal(name, item.Name);
        }

        [Theory]
        [InlineData("  Flashlight  ", "Flashlight")]
        [InlineData("\tFlashlight\n", "Flashlight")]
        public void Ctor_Should_TrimName(string input, string expected)
        {
            var item = new Item(input);

            Assert.Equal(expected, item.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t\r\n")]
        public void Ctor_Should_Throw_WhenNameIsNullOrWhitespace(string? input)
        {
            Assert.Throws<ArgumentException>(() => new Item(input!));
        }

        [Fact]
        public void Rename_Should_UpdateName()
        {
            var item = new Item("Old");

            item.Rename("New");

            Assert.Equal("New", item.Name);
        }

        [Theory]
        [InlineData("  New  ", "New")]
        public void Rename_Should_TrimName(string input, string expected)
        {
            var item = new Item("Old");

            item.Rename(input);

            Assert.Equal(expected, item.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Rename_Should_Throw_WhenNameIsNullOrWhitespace(string? input)
        {
            var item = new Item("Ok");

            Assert.Throws<ArgumentException>(() => item.Rename(input!));
        }

        [Fact]
        public void LocationId_Should_BeNull_ByDefault()
        {
            var item = new Item("Ok");

            Assert.Null(item.LocationId);
        }

       
        [Fact]
        public void MoveToLocation_Should_SetLocationId()
        {
            var item = new Item("Ok");
            var locationId = Guid.NewGuid();
        
            item.MoveToLocation(locationId);
        
            Assert.Equal(locationId, item.LocationId);
        }
        
        [Fact]
        public void MoveToLocation_Should_Allow_Null_ForUnassigned()
        {
            var item = new Item("Ok");
            var locationId = Guid.NewGuid();
            item.MoveToLocation(locationId);
        
            item.MoveToLocation(null);
        
            Assert.Null(item.LocationId);
        }
    }
}
