using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace HomeInventory.Domain.Tests
{
    public class LocationTests
    {

        [Theory]
        [InlineData("   test", "test")]
        [InlineData("test    ", "test")]
        [InlineData("   test    ", "test")]
        public void Ctor_Trimming_FixedName(string name, string expected)
        {
            Location location = new(name, Guid.NewGuid(), Guid.NewGuid());
            Assert.Equal(expected, location.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Ctor_InvalidName_ThrowsArgumentException(string name)
        {            
            Assert.Throws<ArgumentException>(() => new Location(name, Guid.NewGuid(), Guid.NewGuid()));
        }

        [Theory]
        [InlineData("name", "name")]
        [InlineData("  name", "name")]
        [InlineData("name  ", "name")]
        [InlineData("  name  ", "name")]
        public void Rename_ValidInput_LocationRenamed(string newName, string expected)
        {
            Location location = new("test_name", Guid.NewGuid(), Guid.NewGuid());
            location.Rename(newName);
            Assert.Equal(expected, location.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Rename_InvalidInput_ThrowsArgumentException(string newName)
        {
            Location location = new("test_name", Guid.NewGuid(), Guid.NewGuid());
            Assert.Throws<ArgumentException>(() => location.Rename(newName));
        }

        [Theory]
        [InlineData("name")]
        [InlineData("  name  ")]
        [InlineData(null)]
        public void SetDescription_ValidInput_DescriptionSet(string? name)
        {
            string? test_name = null;
            if (name is not null)
                test_name = name.Trim();
            Location location = new("test", Guid.NewGuid(), Guid.NewGuid());
            location.SetDescription(name);
            Assert.Equal(test_name, location.Description);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void SetDescription_InvalidInput_ThrowsArgumentException(string desc)
        {
            Location location = new("test", Guid.NewGuid(), Guid.NewGuid());
            Assert.Throws<ArgumentException>(() => location.SetDescription(desc));
        }

        [Fact]
        public void MoveTo_ValidInput_ParentChanged()
        {
            Guid newParentId = Guid.NewGuid();
            Location location = new("test_name", Guid.NewGuid(), Guid.NewGuid());
            location.MoveTo(newParentId);
            Assert.Equal(newParentId, location.ParentLocationId);
        }

        [Fact]
        public void MoveTo_ValidInputNull_ParentChanged()
        {
            Location location = new("test_name", Guid.NewGuid(), Guid.NewGuid());
            location.MoveTo(null);
            Assert.Null(location.ParentLocationId);
        }

        [Fact]
        public void MoveTo_Self_ThrowsArgumentException()
        {
            Location location = new("test_name", Guid.NewGuid(), Guid.NewGuid());
            Assert.Throws<ArgumentException>(() => location.MoveTo(location.Id));
        }
    }
}
