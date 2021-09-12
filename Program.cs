using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OsuBeatmapsComparer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var firstFileDirectory = Console.ReadLine();
            var secondFileDirectory = Console.ReadLine();

            OsuParsers.Database.OsuDatabase firstDb = OsuParsers.Decoders.DatabaseDecoder.DecodeOsu(firstFileDirectory);
            OsuParsers.Database.OsuDatabase secondDb = OsuParsers.Decoders.DatabaseDecoder.DecodeOsu(secondFileDirectory);

            Console.WriteLine("첫번째 데이터파일의 비트맵 카운트:" + firstDb.BeatmapCount);
            Console.WriteLine("두번째 데이터파일의 비트맵 카운트:" + secondDb.BeatmapCount);


            List<OsuParsers.Database.Objects.DbBeatmap> sameBeatmapSets = new List<OsuParsers.Database.Objects.DbBeatmap>();
            List<OsuParsers.Database.Objects.DbBeatmap> diffrentBeatmapSets = new List<OsuParsers.Database.Objects.DbBeatmap>();

            if (firstDb.BeatmapCount >= secondDb.BeatmapCount)
            {
                for(int i = 0; i < firstDb.BeatmapCount; i++)
                {
                    for (int j = 0; j < secondDb.BeatmapCount; j++)
                    {
                        if(firstDb.Beatmaps[i].BeatmapId == secondDb.Beatmaps[j].BeatmapId)
                        {
                            if (sameBeatmapSets.Find(s => s.BeatmapSetId == secondDb.Beatmaps[j].BeatmapSetId) == null)
                                sameBeatmapSets.Add(secondDb.Beatmaps[j]);
                        }
                        else
                        {
                            if (diffrentBeatmapSets.Find(s => s.BeatmapSetId == secondDb.Beatmaps[j].BeatmapSetId) == null)
                                diffrentBeatmapSets.Add(secondDb.Beatmaps[j]);
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
                            if (sameBeatmapSets.Find(s => s.BeatmapSetId == firstDb.Beatmaps[j].BeatmapSetId) == null)
                                sameBeatmapSets.Add(firstDb.Beatmaps[j]);
                        }
                        else
                        {
                            if (diffrentBeatmapSets.Find(s => s.BeatmapSetId == secondDb.Beatmaps[j].BeatmapSetId) == null)
                                diffrentBeatmapSets.Add(secondDb.Beatmaps[j]);
                        }
                    }
                }
            }

            Console.WriteLine("같은 비트맵셋이 " + sameBeatmapSets.Count + " 개 있습니다");
            Console.WriteLine("다른 비트맵셋이 " + diffrentBeatmapSets.Count + " 개 있습니다");

            var stream = System.IO.File.CreateText(System.IO.Directory.GetCurrentDirectory()+"\\beatmapLists.txt");

            stream.WriteLine("-------같은 비트맵셋 리스트 시작-------");
            foreach(var bm in sameBeatmapSets)
            {
                stream.WriteLine(bm.BeatmapSetId + " : " + bm.Title);
            }

            stream.WriteLine("\n-------다른 비트맵셋 리스트 시작-------");
            foreach (var bm in diffrentBeatmapSets)
            {
                stream.WriteLine(bm.BeatmapSetId + " : " + bm.Title);
            }
            stream.Close();

            Console.WriteLine("아무키나 눌러 다운로드를 시작합니다");
            Console.ReadLine();

            // await AddBeatmap(1115477);
        }

        private static string BaseUrl => @"https://osu.ppy.sh/beatmapsets/";

        public static async Task<bool> AddBeatmap(uint id)
        {
            using (var client = new HttpClient())
            {
                Trace.Write($"Downloading beatmap '{BaseUrl}{id}'...\t");

                var dlTimer = new Stopwatch();
                dlTimer.Start();
                byte[] data = await client.GetByteArrayAsync(new Uri(BaseUrl + id + "/download"));
                dlTimer.Stop();

                var file = Encoding.UTF8.GetString(data, 0, data.Length);
                var lines = file.Split("\r\n".Split(), StringSplitOptions.None);

                if (data.Length == 0)
                {
                    Trace.WriteLine($"Failed, file empty ({data.Length}B)");
                    return false;
                }
                else
                {
                    Trace.WriteLine($"Completed in {dlTimer.ElapsedMilliseconds}ms ({Math.Round((double)data.Length / 1024d, 3)}KB)");
                    //RawFiles.Add(lines);
                    return true;
                }
            }
        }
    }
}
