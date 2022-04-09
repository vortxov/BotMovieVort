using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTorrent.Api;

namespace BotMovieVort.Service
{
    public class UTorrentService
    {
        public static List<string> DownloadTorrent(string path)
        {
            try
            {
                int time = 0;
                var listPath = new List<string>();
                using (var file = System.IO.File.OpenRead(path))
                {
                    UTorrentClient client = new UTorrentClient("admin", "admin");
                    var response = client.PostTorrent(file, "tools");
                    var torrent = response.AddedTorrent;
                    var filesTorrent = client.GetTorrent(torrent.Hash);


                    foreach (var fileTorrent in filesTorrent.Result.Files.Values)
                    {
                        foreach (var item in fileTorrent)
                        {
                            listPath.Add(torrent.Path + "\\" + item.Name);
                        }
                    }

                    while (true)   //TODO: Проверка на mp4 
                    {
                        bool progress = true;
                        var tr = client.GetTorrent(torrent.Hash);
                        foreach (var values in tr.Result.Files.Values)
                        {
                            foreach (var item in values)
                            {
                                if (item.Progress != 100)
                                {
                                    progress = false;
                                    break;
                                }
                            }
                            if (!progress)
                            {
                                break;
                            }
                        }

                        if(time == 100)
                        {
                            client.Remove(torrent.Hash);
                            return null;
                        }


                        if (progress)
                        {
                            client.Remove(torrent.Hash);
                            break;
                        }
                        else
                        {
                            Thread.Sleep(10000);
                            time++;
                        }
                    }
                }
                return listPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
