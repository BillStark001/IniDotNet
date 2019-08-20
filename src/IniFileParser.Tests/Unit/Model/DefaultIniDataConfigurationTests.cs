﻿using System;
using System.Collections.Generic;
using IniParser.Model;
using IniParser.Model.Configuration;
using NUnit.Framework;

namespace IniFileParser.Tests.Unit.Model
{
    [TestFixture, Category("Test of data structures used to hold information retrieved for an INI file")]
    public class IniDataConfigurationTests
    {
        [Test]
        public void check_default_values()
        {
            var config = new IniParserConfiguration();

            Assert.That(config, Is.Not.Null);
        }

        [Test]
        public void check_cloning()
        {
            IniParserConfiguration config1 = new IniParserConfiguration();

            config1.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepFirstValue;

			Assert.That(config1.DuplicatePropertiesBehaviour, 
                        Is.EqualTo(IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepFirstValue));

			IniParserConfiguration config2 = config1.DeepClone();

            Assert.That(config2.DuplicatePropertiesBehaviour,
                        Is.EqualTo(IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepFirstValue));
        }

        [Test]
        public void create_key_with_invalid_name()
        {
            Assert.Throws( typeof(ArgumentException), () => new Property(""));
        }

        [Test]
        public void creating_keydata_programatically()
        {

            var strValueTest = "Test String";
            var strKeyTest = "Mykey";
            var commentListTest = new List<string>(new string[] { "testComment 1", "testComment 2" });

            //Create a key data
            Property kd = new Property(strKeyTest);
            kd.Value = strValueTest;
            kd.Comments = commentListTest;
            
            //Assert not null and empty
            Assert.That(kd, Is.Not.Null);
            Assert.That(kd.KeyName, Is.EqualTo(strKeyTest));
            Assert.That(kd.Value, Is.EqualTo(strValueTest));
            Assert.That(kd.Comments, Has.Count.EqualTo(2));
            Assert.That(kd.Comments[0], Is.EqualTo("testComment 1"));
            Assert.That(kd.Comments[1], Is.EqualTo("testComment 2"));

        }

        [Test]
        public void check_clone_copies_data()
        {
            var strValueTest = "Test String";
            var strKeyTest = "Mykey";
            var commentListTest = new List<string>(new string[] { "testComment 1", "testComment 2" });

            //Create a key data
            Property kd2 = new Property(strKeyTest);
            kd2.Value = strValueTest;
            kd2.Comments = commentListTest;

            Property kd = kd2.DeepClone();


            //Assert not null and empty
            Assert.That(kd, Is.Not.Null);
            Assert.That(kd.KeyName, Is.EqualTo(strKeyTest));
            Assert.That(kd.Value, Is.EqualTo(strValueTest));
            Assert.That(kd.Comments, Has.Count.EqualTo(2));
            Assert.That(kd.Comments[0], Is.EqualTo("testComment 1"));
            Assert.That(kd.Comments[1], Is.EqualTo("testComment 2"));


            kd.Value = "t";
            Assert.That(kd2.Value, Is.EqualTo(strValueTest));
            Assert.That(kd.Value, Is.EqualTo("t"));

        }
         
    }
}