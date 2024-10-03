using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace update_stash_api.Services.DTOs
{
    public class FindJobResponseType
    {
        public JobStatus findJob { get; set; }
    }

    public class JobStatus
    {
        public string id { get; set; }
        public string status { get; set; }
    }

    public class QueueMetadataIdentifyJobInputFieldOption
    {
        public string field { get; set; }
        public string strategy { get; set; }
        public bool createMissing { get; set; }
    }

    public class QueueMetadataIdentifyJobInputOptions
    {
        public List<QueueMetadataIdentifyJobInputFieldOption> fieldOptions { get; set; }
        public bool setCoverImage { get; set; } = true;
        public bool setOrganized { get; set; } = false;
        public bool includeMalePerformers { get; set; } = false;
        public bool skipMultipleMatches { get; set; } = true;
        public object skipMultipleMatchTag { get; set; } = null;
        public bool skipSingleNamePerformers { get; set; } = true;
        public object skipSingleNamePerformerTag { get; set; } = null;
    }

    public class QueueMetadataIdentifyJobInput
    {
        public List<QueueMetadataIdentifyJobInputScraperSource> sources { get; set; }
        public QueueMetadataIdentifyJobInputOptions options { get; set; }
        public List<string> paths { get; set; }
    }

    public class QueueMetadataIdentifyJobInputScraperSource
    {
        public QueueMetadataIdentifyJobInputScraperSource2 source { get; set; }
    }

    public class QueueMetadataIdentifyJobInputScraperSource2
    {
        public string scraper_id { get; set; }
    }

    public class QueueMetadataIdentifyJobResponseType
    {
        public string metadataIdentify { get; set; }
    }

    public class QueueMetadataScanJobResponseType
    {
        public string metadataScan { get; set; }
    }

    public class QueueMetadataScanJobInput
    {
        public bool scanGenerateCovers { get; set; } = true;
        public bool scanGeneratePreviews { get; set; } = true;
        public bool scanGenerateImagePreviews { get; set; } = true;
        public bool scanGenerateSprites { get; set; } = true;
        public bool scanGeneratePhashes { get; set; } = true;
        public bool scanGenerateThumbnails { get; set; } = true;
        public bool scanGenerateClipPreviews { get; set; } = true;
        public List<string> paths { get; set; }
    }

    public class SceneQueryResponseType
    {
        public FindScenesList findScenes { get; set; }
    }

    public class FindScenesList
    {
        public int count { get; set; }
        public Scene[] scenes { get; set; }
    }

    public class ScrapeSceneURLQueryResponseType
    {
        public Scene scrapeSceneURL { get; set; }
    }

    public class Scene
    {
        public string id { get; set; }
        public string title { get; set; }
        public string code { get; set; }
        public string details { get; set; }
        public string[] urls { get; set; }
        public string date { get; set; }
        public string image { get; set; }
        public SceneFile[] files { get; set; }

        public override string ToString()
        {
            return $"{code}";
        }
    }

    public class SceneFile
    {
        public string id { get; set; }
        public string path { get; set; }
        public long size { get; set; }
        public DateTimeOffset mod_time { get; set; }

        [JsonIgnore]
        public string filename => Path.GetFileNameWithoutExtension(path ?? string.Empty);

        public override string ToString() => filename;
    }

    public class SceneUpdateInput
    {
        public string id { get; set; }
        public string title { get; set; }
        public string code { get; set; }
        public string details { get; set; }
        public string[] urls { get; set; }
        public string cover_image { get; set; }
        public string date { get; set; }
    }

    public class SceneUpdateResponseType
    {
        public Scene sceneUpdate { get; set; }
    }

    public class ScrapeSingleSceneQueryResponseType
    {
        public Scene[] scrapeSingleScene { get; set; }
    }

    public static class JavIdFormatter
    {

        static string captureNormalJavIDpattern = @"([a-zA-Z]+-\d+)";
        static string captureJavIDspecialCase1pattern = @"([a-zA-Z]+0{1,2}\d+)";
        static string reformatJavIDspecialCase1pattern = @"([a-zA-Z]+)(0{1,2})(\d+)";
        static string captureJavIDpartNumberpattern = @"(\d+)$";
        static Regex captureNormalJavID = new Regex(captureNormalJavIDpattern);
        static Regex captureJavIDspecialCase1 = new Regex(captureJavIDspecialCase1pattern);
        static Regex captureJavIDpartNumber = new Regex(captureJavIDpartNumberpattern);

        public static Scene FormatJavId(string filename, string id)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return null;
            }

            MatchCollection matches;
            if (filename.Contains("vr"))
            {
                matches = captureJavIDspecialCase1.Matches(filename);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        var code = Regex.Replace(match.Groups[1].Value, reformatJavIDspecialCase1pattern, "$1-$3").ToUpper();
                        matches = captureJavIDpartNumber.Matches(filename.Replace("_8k", "").Replace("-8k", "").Replace("_A", "_1").Replace("_B", "_2").Replace("_C", "_3").Replace(" A", "_1").Replace(" B", "_2").Replace(" C", "_3").Replace("-A", "_1").Replace("-B", "_2").Replace("-C", "_3").Replace("-icao.me", "").Replace(" - Virtual Reality - Jav - XFantazy.com", "").Replace(" - Virtual Reality - Virtual reality - XFantazy.com", "").Replace(" - Virtual Reality - Smartphone - XFantazy.com", ""));
                        if (matches.Count > 0)
                        {
                            foreach (Match match1 in matches)
                            {
                                var part = match1.Groups[1].Value;
                                //check if part is just the code
                                if (!code.EndsWith(part))
                                {
                                    return new Scene()
                                    {
                                        id = id,
                                        code = code,
                                        title = $"{code}-{part}",
                                    };
                                }

                                return new Scene()
                                {
                                    id = id,
                                    code = code,
                                    title = code,
                                };
                            }
                        }
                        return new Scene()
                        {
                            id = id,
                            code = code,
                            title = code,
                        };
                    }
                }
                else
                {
                    matches = captureNormalJavID.Matches(filename);
                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            var code = match.Groups[1].Value.ToUpper();
                            matches = captureJavIDpartNumber.Matches(filename.Replace("_8k", "").Replace("_A", "_1").Replace("_B", "_2").Replace("_C", "_3").Replace(" A", "_1").Replace(" B", "_2").Replace(" C", "_3").Replace("-A", "_1").Replace("-B", "_2").Replace("-C", "_3").Replace("-icao.me", "").Replace(" - Virtual Reality - Jav - XFantazy.com", "").Replace(" - Virtual Reality - Virtual reality - XFantazy.com", "").Replace(" - Virtual Reality - Smartphone - XFantazy.com", ""));
                            if (matches.Count > 0)
                            {
                                foreach (Match match1 in matches)
                                {
                                    var part = match1.Groups[1].Value;
                                    //check if part is just the code
                                    if (!code.EndsWith(part))
                                    {
                                        return new Scene()
                                        {
                                            id = id,
                                            code = code,
                                            title = $"{code}-{part}",
                                        };
                                    }

                                    return new Scene()
                                    {
                                        id = id,
                                        code = code,
                                        title = code,
                                    };
                                }
                            }
                            return new Scene()
                            {
                                id = id,
                                code = code,
                                title = code,
                            };
                        }
                    }
                }
            }
            else
            {
                matches = captureNormalJavID.Matches(filename);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        var code = match.Groups[1].Value.ToUpper();
                        matches = captureJavIDpartNumber.Matches(filename.Replace("_8k", "").Replace("_A", "_1").Replace("_B", "_2").Replace("_C", "_3").Replace(" A", "_1").Replace(" B", "_2").Replace(" C", "_3").Replace("-A", "_1").Replace("-B", "_2").Replace("-C", "_3").Replace("-icao.me", "").Replace(" - Virtual Reality - Jav - XFantazy.com", "").Replace(" - Virtual Reality - Virtual reality - XFantazy.com", "").Replace(" - Virtual Reality - Smartphone - XFantazy.com", ""));
                        if (matches.Count > 0)
                        {
                            foreach (Match match1 in matches)
                            {
                                var part = match1.Groups[1].Value;
                                //check if part is just the code
                                if (!code.EndsWith(part))
                                {
                                    return new Scene()
                                    {
                                        id = id,
                                        code = code,
                                        title = $"{code}-{part}",
                                    };
                                }

                                return new Scene()
                                {
                                    id = id,
                                    code = code,
                                    title = code,
                                };
                            }
                        }
                        return new Scene()
                        {
                            id = id,
                            code = code,
                            title = code,
                        };
                    }
                }
            }

            return null;
        }
    }
}
