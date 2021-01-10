using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using DotsGame.Sgf;

namespace DotsGame.Formats
{
    public class GameInfoExtractor
    {
        private const string VkPlaydotsSgfPrefix = "https://game.playdots.ru/export/sgf/{0}/{1}";

        private static Tuple<string, GameInfo> CachedGameInfo = new Tuple<string, GameInfo>(null, null);

        public static void InvalidateCache()
        {
            CachedGameInfo = new Tuple<string, GameInfo>(null, null);
        }

        public GameInfo DetectFormatAndOpen(string fileUrlOrPath)
        {
            return DetectFormatAndOpen(fileUrlOrPath, out bool fromCache);
        }

        public GameInfo DetectFormatAndOpen(string fileUrlOrPath, out bool fromCache)
        {
            fileUrlOrPath = fileUrlOrPath.Trim();
            GameInfo result = null;
            byte[] data = null;
            IDotsGameFormatParser parser = null;
            bool fromUrl = false;
            if (fileUrlOrPath.StartsWith("http://") || fileUrlOrPath.StartsWith("https://") ||
                long.TryParse(fileUrlOrPath, out long vkId))
            {
                int digitPos = fileUrlOrPath.Length - 1;
                while (digitPos >= 0 && char.IsDigit(fileUrlOrPath[digitPos]))
                {
                    digitPos--;
                }
                digitPos++;
                vkId = long.Parse(fileUrlOrPath.Substring(digitPos));
                
                List<string> sgfUrls = new List<string>();
                if (fileUrlOrPath.Contains("/game/"))
                {
                    sgfUrls.Add(string.Format(VkPlaydotsSgfPrefix, "game", vkId));
                }
                else if (fileUrlOrPath.Contains("/practice/"))
                {
                    sgfUrls.Add(string.Format(VkPlaydotsSgfPrefix, "practice", vkId));
                }
                else
                {
                    sgfUrls.Add(string.Format(VkPlaydotsSgfPrefix, "game", vkId));
                    sgfUrls.Add(string.Format(VkPlaydotsSgfPrefix, "practice", vkId));
                }
                var webClient = new WebClient();
                webClient.Headers["Accept-Language"] = "en-US";
                Exception lastException = null;
                foreach (var sgfUrl in sgfUrls)
                {
                    try
                    {
                        data = webClient.DownloadData(sgfUrl);
                        if (Encoding.Default.GetString(data) != "An error occurred, it could not be saved in the format of sgf")
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                    }
                }
                if (data == null)
                {
                    throw lastException;
                }
                parser = new SgfParser();
                fromUrl = true;
            }
            else if (Path.GetExtension(fileUrlOrPath) == ".sav")
            {
                parser = new PointsXtParser();
                data = File.ReadAllBytes(fileUrlOrPath);
            }
            else if (Path.GetExtension(fileUrlOrPath) == ".sgf")
            {
                parser = new SgfParser();
                data = File.ReadAllBytes(fileUrlOrPath);
            }
            else
            {
                throw new NotSupportedException($"Format of file {fileUrlOrPath} can not be detected or not supported");
            }

            string hash = CalculateHash(data);
            if (CachedGameInfo.Item1 != hash)
            {
                result = parser.Parse(data);
                result.FromUrl = fromUrl;
                CachedGameInfo = new Tuple<string, GameInfo>(hash, result);
                fromCache = false;
            }
            else
            {
                result = CachedGameInfo.Item2;
                fromCache = true;
            }

            return result;
        }

        private string CalculateHash(byte[] data)
        {
            string result;
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                result = BitConverter.ToString(sha1.ComputeHash(data));
            }
            return result;
        }
    }
}
