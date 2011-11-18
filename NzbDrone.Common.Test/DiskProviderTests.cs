﻿// ReSharper disable InconsistentNaming
using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Test.Common;

namespace NzbDrone.Common.Test
{
    [TestFixture]
    public class DiskProviderTests : TestBase
    {
        DirectoryInfo BinFolder;
        DirectoryInfo BinFolderCopy;
        DirectoryInfo BinFolderMove;

        [SetUp]
        public void Setup()
        {
            var binRoot = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent;
            BinFolder = new DirectoryInfo(Path.Combine(binRoot.FullName, "bin"));
            BinFolderCopy = new DirectoryInfo(Path.Combine(binRoot.FullName, "bin_copy"));
            BinFolderMove = new DirectoryInfo(Path.Combine(binRoot.FullName, "bin_move"));

            if (BinFolderCopy.Exists)
            {
                BinFolderCopy.Delete(true);
            }

            if (BinFolderMove.Exists)
            {
                BinFolderMove.Delete(true);
            }
        }

        [Test]
        public void CopyFolder_should_copy_folder()
        {
            //Act
            var diskProvider = new DiskProvider();
            diskProvider.CopyDirectory(BinFolder.FullName, BinFolderCopy.FullName);

            //Assert
            VerifyCopy();
        }

        [Test]
        public void CopyFolder_should_overright_existing_folder()
        {
            //Act
            var diskProvider = new DiskProvider();

            diskProvider.CopyDirectory(BinFolder.FullName, BinFolderCopy.FullName);

            //Delete Random File
            BinFolderCopy.Refresh();
            BinFolderCopy.GetFiles("*.*", SearchOption.AllDirectories).First().Delete();

            diskProvider.CopyDirectory(BinFolder.FullName, BinFolderCopy.FullName);

            //Assert
            VerifyCopy();
        }

        [Test]
        public void MoveFolder_should_overright_existing_folder()
        {
            var diskProvider = new DiskProvider();

            diskProvider.CopyDirectory(BinFolder.FullName, BinFolderCopy.FullName);
            diskProvider.CopyDirectory(BinFolder.FullName, BinFolderMove.FullName);
            VerifyCopy();

            //Act
            diskProvider.MoveDirectory(BinFolderCopy.FullName, BinFolderMove.FullName);

            //Assert
            VerifyMove();
        }

        private void VerifyCopy()
        {
            BinFolder.Refresh();
            BinFolderCopy.Refresh();

            BinFolderCopy.GetFiles("*.*", SearchOption.AllDirectories)
               .Should().HaveSameCount(BinFolder.GetFiles("*.*", SearchOption.AllDirectories));

            BinFolderCopy.GetDirectories().Should().HaveSameCount(BinFolder.GetDirectories());
        }

        private void VerifyMove()
        {
            BinFolder.Refresh();
            BinFolderCopy.Refresh();
            BinFolderMove.Refresh();

            BinFolderCopy.Exists.Should().BeFalse();

            BinFolderMove.GetFiles("*.*", SearchOption.AllDirectories)
               .Should().HaveSameCount(BinFolder.GetFiles("*.*", SearchOption.AllDirectories));

            BinFolderMove.GetDirectories().Should().HaveSameCount(BinFolder.GetDirectories());
        }
    }
}
