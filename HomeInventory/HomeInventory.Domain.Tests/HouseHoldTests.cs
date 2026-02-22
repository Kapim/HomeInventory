using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace HomeInventory.Domain.Tests
{
    public class HouseholdTests
    {

        [Theory]
        [InlineData("   test", "test")]
        [InlineData("test    ", "test")]
        [InlineData("   test    ", "test")]
        public void Ctor_Trimming_FixedName(string name, string expected)
        {
            Household household = new(name);
            Assert.Equal(expected, household.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Ctor_InvalidName_ThrowsArgumentException(string name)
        {            
            Assert.Throws<ArgumentException>(() => new Household(name));
        }

        [Theory]
        [InlineData("name", "name")]
        [InlineData("  name", "name")]
        [InlineData("name  ", "name")]
        [InlineData("  name  ", "name")]
        public void Rename_ValidInput_HouseholdRenamed(string newName, string expected)
        {
            Household household = new("test_name");
            household.Rename(newName);
            Assert.Equal(expected, household.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Rename_InvalidInput_ThrowsArgumentException(string newName)
        {
            Household household = new("test_name");
            Assert.Throws<ArgumentException>(() => household.Rename(newName));
        }
    }
}
