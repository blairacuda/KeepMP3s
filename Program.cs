using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepMP3s
{
    class Program
    {
        private const string DIR_MUSIC = @"P:\music";
        private const string DIR_TRASH = @"P:\!trash\music";
        private const string EXT_WMA = ".wma";
        private const string EXT_MP3 = ".mp3";

        private static int _trashSongs;

        static void Main(string[] args)
        {
            Console.WriteLine("Hit any key to begin");
            Console.ReadKey();

            if (!Directory.Exists(DIR_MUSIC))
            {
                Console.WriteLine("Cannot find folder. Hit any key to end.");
                Console.ReadKey();
                return;
            }

            RunThroughFolders(DIR_MUSIC);


            Console.WriteLine($"Finished. Moved {_trashSongs} songs to trash.");
            Console.ReadKey();
        }

        private static void RunThroughFolders(string directory)
        {
            try
            {
                Parallel.ForEach(Directory.EnumerateDirectories(directory), (dir) =>
                    {
                        MoveWMADupesToTrash(dir);
                        RunThroughFolders(dir);
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Loop - {ex}");
            }
        }

        private static void MoveWMADupesToTrash(string directory)
        {
            try
            {
                var files = Directory.GetFiles(directory);
                var wmas = files.Where(file => Path.GetExtension(file).Equals(EXT_WMA, StringComparison.InvariantCultureIgnoreCase)).ToList();
                if (wmas.Count == 0) return;

                var mp3s = files.Where(file => Path.GetExtension(file).Equals(EXT_MP3, StringComparison.InvariantCultureIgnoreCase)).ToList();
                if (mp3s.Count == 0) return;

                Parallel.ForEach(mp3s, (mp3) =>
                {
                    var foundSong = wmas.FirstOrDefault(wma => Path.GetFileNameWithoutExtension(wma).Equals(Path.GetFileNameWithoutExtension(mp3)));
                    if (foundSong != null)
                    {
                        Console.WriteLine($"Moving song to trash - {foundSong}");
                        _trashSongs++;
                        var trashSong = foundSong.Replace(DIR_MUSIC, DIR_TRASH);
                        MoveSong(foundSong, trashSong);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Move error - {ex}");
                Console.ReadKey();
            }
        }

        private static void MoveSong(string sourceFile, string targetFile)
        {
            try
            {
                var targetDir = Path.GetDirectoryName(targetFile);
                if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
                File.Move(sourceFile, targetFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during move - {ex}");
            }

        }
    }
}
