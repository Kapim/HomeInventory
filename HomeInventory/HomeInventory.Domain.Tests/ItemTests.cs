using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Domain.Tests
{
    public class ItemTests
    {
        readonly Guid _ownerId = Guid.NewGuid();
        readonly Guid _locationId = Guid.NewGuid();
        [Fact]
        public void Ctor_Should_SetName()
        {
            var name = "Flashlight";
            var item = new Item(name, _ownerId, _locationId);
            Assert.Equal(name, item.Name);
        }

        [Theory]
        [InlineData("  Flashlight  ", "Flashlight")]
        [InlineData("\tFlashlight\n", "Flashlight")]
        public void Ctor_Should_TrimName(string input, string expected)
        {
            var item = new Item(input, _ownerId, _locationId);

            Assert.Equal(expected, item.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t\r\n")]
        public void Ctor_Should_Throw_WhenNameIsNullOrWhitespace(string? input)
        {
            Assert.Throws<ArgumentException>(() => new Item(input!, _ownerId, _locationId));
        }

        [Fact]
        public void Rename_Should_UpdateName()
        {
            var item = new Item("Old", _ownerId, _locationId);

            item.Rename("New");

            Assert.Equal("New", item.Name);
        }

        [Theory]
        [InlineData("  New  ", "New")]
        public void Rename_Should_TrimName(string input, string expected)
        {
            var item = new Item("Old", _ownerId, _locationId);

            item.Rename(input);

            Assert.Equal(expected, item.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Rename_Should_Throw_WhenNameIsNullOrWhitespace(string? input)
        {
            var item = new Item("Ok", _ownerId, _locationId);

            Assert.Throws<ArgumentException>(() => item.Rename(input!));
        }

              
        [Fact]
        public void MoveToLocation_Should_SetLocationId()
        {
            var item = new Item("Ok", _ownerId, _locationId);
            var locationId = Guid.NewGuid();
        
            item.MoveToLocation(locationId);
        
            Assert.Equal(locationId, item.LocationId);
        }
        
       
    }
}
