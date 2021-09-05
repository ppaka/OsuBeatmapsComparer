using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsuParsers;

namespace OsuBeatmapsComparer
{
    class Program
    {
        static void Main(string[] args)
        {
            var firstFileDirectory = Console.ReadLine();
            var secondFileDirectory = Console.ReadLine();

            OsuParsers.Database.OsuDatabase firstDb = OsuParsers.Decoders.DatabaseDecoder.DecodeOsu(firstFileDirectory);
            OsuParsers.Database.OsuDatabase secondDb = OsuParsers.Decoders.DatabaseDecoder.DecodeOsu(secondFileDirectory);

            Console.WriteLine("첫번째 데이터파일의 비트맵 카운트:" + firstDb.BeatmapCount);
            Console.WriteLine("두번째 데이터파일의 비트맵 카운트:" + secondDb.BeatmapCount);


            List<OsuParsers.Database.Objects.DbBeatmap> sameBeatmaps = new List<OsuParsers.Database.Objects.DbBeatmap>();
            List<OsuParsers.Database.Objects.DbBeatmap> diffrentBeatmaps = new List<OsuParsers.Database.Objects.DbBeatmap>();

            if (firstDb.BeatmapCount >= secondDb.BeatmapCount)
            {
                for(int i = 0; i < firstDb.BeatmapCount; i++)
                {
                    for (int j = 0; j < secondDb.BeatmapCount; j++)
                    {
                        if(firstDb.Beatmaps[i].BeatmapId == secondDb.Beatmaps[j].BeatmapId)
                        {
                            sameBeatmaps.Add(secondDb.Beatmaps[j]);
                        }
                        else
                        {
                            diffrentBeatmaps.Add(secondDb.Beatmaps[j]);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < secondDb.BeatmapCount; i++)
                {
                    for (int j = 0; j < firstDb.BeatmapCount; j++)
                    {
                        if (secondDb.Beatmaps[i].BeatmapId == firstDb.Beatmaps[j].BeatmapId)
                        {
                            sameBeatmaps.Add(firstDb.Beatmaps[j]);
                        }
                        else
                        {
                            diffrentBeatmaps.Add(secondDb.Beatmaps[j]);
                        }
                    }
                }
            }

            Console.WriteLine("같은 비트맵이 " + sameBeatmaps.Count + " 개 있습니다");
            Console.WriteLine("다른 비트맵이 " + diffrentBeatmaps.Count + " 개 있습니다");

            var stream = System.IO.File.CreateText(System.IO.Directory.GetCurrentDirectory()+"\\beatmapLists.txt");

            stream.WriteLine("-------같은 비트맵 리스트 시작-------");
            foreach(var bm in sameBeatmaps)
            {
                stream.WriteLine(bm.BeatmapId + "/" + bm.Title);
            }

            stream.WriteLine("-------다른 비트맵 리스트 시작-------");
            foreach (var bm in diffrentBeatmaps)
            {
                stream.WriteLine(bm.BeatmapId + "/" + bm.Title);
            }
            stream.Close();
        }
    }
}
