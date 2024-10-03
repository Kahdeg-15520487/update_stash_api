using GraphQL.Client.Http;
using GraphQL;
using update_stash_api.Services.DTOs;
using GraphQL.Client.Serializer.Newtonsoft;

namespace update_stash_api.Services
{
    public class StashGraphQlService : IDisposable, IStashGraphQlService
    {
        private readonly GraphQLHttpClient graphQLClient;

        public StashGraphQlService(IConfiguration configuration)
        {
            graphQLClient = new GraphQLHttpClient(configuration.GetConnectionString("stashurl"), new NewtonsoftJsonSerializer());
            graphQLClient.HttpClient.DefaultRequestHeaders.Add("ApiKey", configuration.GetConnectionString("apikey"));
        }

        public async Task<Scene> ScrapeSceneUrl(string url)
        {
            var query = new GraphQLRequest
            {
                Variables = new
                {
                    url = url
                },
                OperationName = "ScrapeSceneURL",
                Query = """
    query ScrapeSceneURL($url: String!) {
      scrapeSceneURL(url: $url) {
        ...ScrapedSceneData
        __typename
      }
    }

    fragment ScrapedSceneData on ScrapedScene {
      title
      code
      details
      director
      urls
      date
      image
      remote_site_id
      file {
        size
        duration
        video_codec
        audio_codec
        width
        height
        framerate
        bitrate
        __typename
      }
      studio {
        ...ScrapedSceneStudioData
        __typename
      }
      tags {
        ...ScrapedSceneTagData
        __typename
      }
      performers {
        ...ScrapedScenePerformerData
        __typename
      }
      movies {
        ...ScrapedSceneMovieData
        __typename
      }
      fingerprints {
        hash
        algorithm
        duration
        __typename
      }
      __typename
    }

    fragment ScrapedSceneStudioData on ScrapedStudio {
      stored_id
      name
      url
      parent {
        stored_id
        name
        url
        image
        remote_site_id
        __typename
      }
      image
      remote_site_id
      __typename
    }

    fragment ScrapedSceneTagData on ScrapedTag {
      stored_id
      name
      __typename
    }

    fragment ScrapedScenePerformerData on ScrapedPerformer {
      stored_id
      name
      disambiguation
      gender
      url
      twitter
      instagram
      birthdate
      ethnicity
      country
      eye_color
      height
      measurements
      fake_tits
      penis_length
      circumcised
      career_length
      tattoos
      piercings
      aliases
      tags {
        ...ScrapedSceneTagData
        __typename
      }
      remote_site_id
      images
      details
      death_date
      hair_color
      weight
      __typename
    }

    fragment ScrapedSceneMovieData on ScrapedMovie {
      stored_id
      name
      aliases
      duration
      date
      rating
      director
      url
      synopsis
      front_image
      back_image
      studio {
        ...ScrapedMovieStudioData
        __typename
      }
      __typename
    }

    fragment ScrapedMovieStudioData on ScrapedStudio {
      stored_id
      name
      url
      __typename
    }
    """
            };

            var response = await graphQLClient.SendQueryAsync<ScrapeSceneURLQueryResponseType>(query);
            if (response.Errors != null)
            {
                throw new Exception(string.Join(Environment.NewLine, response.Errors.Select(e => $"{e.Message}\r\n{e.Path}\r\n{e.Locations}")));
            }
            return response.Data.scrapeSceneURL;
        }

        public async Task<Scene> ScrapeSceneId(string javId)
        {
            var query = new GraphQLRequest
            {
                Variables = new
                {
                    source = new
                    {
                        scraper_id = "javdb"
                    },
                    input = new
                    {
                        query = javId
                    }
                },
                OperationName = "ScrapeSingleScene",
                Query = """
        query ScrapeSingleScene($source: ScraperSourceInput!, $input: ScrapeSingleSceneInput!) {
      scrapeSingleScene(source: $source, input: $input) {
        ...ScrapedSceneData
        __typename
      }
    }

    fragment ScrapedSceneData on ScrapedScene {
      title
      code
      details
      director
      urls
      date
      image
      remote_site_id
      file {
        size
        duration
        video_codec
        audio_codec
        width
        height
        framerate
        bitrate
        __typename
      }
      studio {
        ...ScrapedSceneStudioData
        __typename
      }
      tags {
        ...ScrapedSceneTagData
        __typename
      }
      performers {
        ...ScrapedScenePerformerData
        __typename
      }
      movies {
        ...ScrapedSceneMovieData
        __typename
      }
      fingerprints {
        hash
        algorithm
        duration
        __typename
      }
      __typename
    }

    fragment ScrapedSceneStudioData on ScrapedStudio {
      stored_id
      name
      url
      parent {
        stored_id
        name
        url
        image
        remote_site_id
        __typename
      }
      image
      remote_site_id
      __typename
    }

    fragment ScrapedSceneTagData on ScrapedTag {
      stored_id
      name
      __typename
    }

    fragment ScrapedScenePerformerData on ScrapedPerformer {
      stored_id
      name
      disambiguation
      gender
      url
      twitter
      instagram
      birthdate
      ethnicity
      country
      eye_color
      height
      measurements
      fake_tits
      penis_length
      circumcised
      career_length
      tattoos
      piercings
      aliases
      tags {
        ...ScrapedSceneTagData
        __typename
      }
      remote_site_id
      images
      details
      death_date
      hair_color
      weight
      __typename
    }

    fragment ScrapedSceneMovieData on ScrapedMovie {
      stored_id
      name
      aliases
      duration
      date
      rating
      director
      url
      synopsis
      front_image
      back_image
      studio {
        ...ScrapedMovieStudioData
        __typename
      }
      __typename
    }

    fragment ScrapedMovieStudioData on ScrapedStudio {
      stored_id
      name
      url
      __typename
    }
    """
            };

            var response = await graphQLClient.SendQueryAsync<ScrapeSingleSceneQueryResponseType>(query);
            if (response.Errors != null)
            {
                throw new Exception(string.Join(Environment.NewLine, response.Errors.Select(e => $"{e.Message}\r\n{e.Path}\r\n{e.Locations}")));
            }
            return response.Data.scrapeSingleScene.Where(s => s.title.Contains(javId)).Select(s =>
            {
                s.details = s.title;
                s.code = javId;
                s.title = javId;
                return s;
            }).FirstOrDefault();
        }

        public async Task<Scene> SceneUpdate(SceneUpdateInput scene)
        {
            var query = new GraphQLRequest
            {
                Variables = new
                {
                    input = new
                    {
                        id = scene.id,
                        title = scene.title,
                        code = scene.code,
                        details = scene.details,
                        urls = scene.urls,
                        cover_image = scene.cover_image,
                        date = scene.date
                    }
                },
                OperationName = "SceneUpdate",
                Query = """
                mutation SceneUpdate($input: SceneUpdateInput!) {
          sceneUpdate(input: $input) {
            ...SceneData
            __typename
          }
        }

        fragment SceneData on Scene {
          id
          title
          code
          details
          director
          urls
          date
          rating100
          o_counter
          organized
          interactive
          interactive_speed
          captions {
            language_code
            caption_type
            __typename
          }
          created_at
          updated_at
          resume_time
          last_played_at
          play_duration
          play_count
          files {
            ...VideoFileData
            __typename
          }
          paths {
            screenshot
            preview
            stream
            webp
            vtt
            sprite
            funscript
            interactive_heatmap
            caption
            __typename
          }
          scene_markers {
            ...SceneMarkerData
            __typename
          }
          galleries {
            ...SlimGalleryData
            __typename
          }
          studio {
            ...SlimStudioData
            __typename
          }
          movies {
            movie {
              ...MovieData
              __typename
            }
            scene_index
            __typename
          }
          tags {
            ...SlimTagData
            __typename
          }
          performers {
            ...PerformerData
            __typename
          }
          stash_ids {
            endpoint
            stash_id
            __typename
          }
          sceneStreams {
            url
            mime_type
            label
            __typename
          }
          __typename
        }

        fragment VideoFileData on VideoFile {
          id
          path
          size
          mod_time
          duration
          video_codec
          audio_codec
          width
          height
          frame_rate
          bit_rate
          fingerprints {
            type
            value
            __typename
          }
          __typename
        }

        fragment SceneMarkerData on SceneMarker {
          id
          title
          seconds
          stream
          preview
          screenshot
          scene {
            id
            __typename
          }
          primary_tag {
            id
            name
            __typename
          }
          tags {
            id
            name
            __typename
          }
          __typename
        }

        fragment SlimGalleryData on Gallery {
          id
          title
          code
          date
          urls
          details
          photographer
          rating100
          organized
          files {
            ...GalleryFileData
            __typename
          }
          folder {
            ...FolderData
            __typename
          }
          image_count
          cover {
            id
            files {
              ...ImageFileData
              __typename
            }
            paths {
              thumbnail
              __typename
            }
            __typename
          }
          chapters {
            id
            title
            image_index
            __typename
          }
          studio {
            id
            name
            image_path
            __typename
          }
          tags {
            id
            name
            __typename
          }
          performers {
            id
            name
            gender
            favorite
            image_path
            __typename
          }
          scenes {
            ...SlimSceneData
            __typename
          }
          __typename
        }

        fragment GalleryFileData on GalleryFile {
          id
          path
          size
          mod_time
          fingerprints {
            type
            value
            __typename
          }
          __typename
        }

        fragment FolderData on Folder {
          id
          path
          __typename
        }

        fragment ImageFileData on ImageFile {
          id
          path
          size
          mod_time
          width
          height
          fingerprints {
            type
            value
            __typename
          }
          __typename
        }

        fragment SlimSceneData on Scene {
          id
          title
          code
          details
          director
          urls
          date
          rating100
          o_counter
          organized
          interactive
          interactive_speed
          resume_time
          play_duration
          play_count
          files {
            ...VideoFileData
            __typename
          }
          paths {
            screenshot
            preview
            stream
            webp
            vtt
            sprite
            funscript
            interactive_heatmap
            caption
            __typename
          }
          scene_markers {
            id
            title
            seconds
            primary_tag {
              id
              name
              __typename
            }
            __typename
          }
          galleries {
            id
            files {
              path
              __typename
            }
            folder {
              path
              __typename
            }
            title
            __typename
          }
          studio {
            id
            name
            image_path
            __typename
          }
          movies {
            movie {
              id
              name
              front_image_path
              __typename
            }
            scene_index
            __typename
          }
          tags {
            id
            name
            __typename
          }
          performers {
            id
            name
            gender
            favorite
            image_path
            __typename
          }
          stash_ids {
            endpoint
            stash_id
            __typename
          }
          __typename
        }

        fragment SlimStudioData on Studio {
          id
          name
          image_path
          stash_ids {
            endpoint
            stash_id
            __typename
          }
          parent_studio {
            id
            __typename
          }
          details
          rating100
          aliases
          __typename
        }

        fragment MovieData on Movie {
          id
          name
          aliases
          duration
          date
          rating100
          director
          studio {
            ...SlimStudioData
            __typename
          }
          synopsis
          url
          front_image_path
          back_image_path
          scene_count
          scenes {
            id
            title
            __typename
          }
          __typename
        }

        fragment SlimTagData on Tag {
          id
          name
          aliases
          image_path
          parent_count
          child_count
          __typename
        }

        fragment PerformerData on Performer {
          id
          name
          disambiguation
          url
          gender
          twitter
          instagram
          birthdate
          ethnicity
          country
          eye_color
          height_cm
          measurements
          fake_tits
          penis_length
          circumcised
          career_length
          tattoos
          piercings
          alias_list
          favorite
          ignore_auto_tag
          image_path
          scene_count
          image_count
          gallery_count
          movie_count
          performer_count
          o_counter
          tags {
            ...SlimTagData
            __typename
          }
          stash_ids {
            stash_id
            endpoint
            __typename
          }
          rating100
          details
          death_date
          hair_color
          weight
          __typename
        }
        """
            };
            var response = await graphQLClient.SendQueryAsync<SceneUpdateResponseType>(query);
            if (response.Errors != null)
            {
                throw new Exception(string.Join(Environment.NewLine, response.Errors.Select(e => $"{e.Message}\r\n{e.Path}\r\n{e.Locations}")));
            }
            return response.Data.sceneUpdate;
        }

        public async Task<FindScenesList> GetAllScenes(string fragmentQuery = "")
        {
            var query = new GraphQLRequest
            {
                Variables = new
                {
                    filter = new
                    {
                        q = fragmentQuery,
                        per_page = -1,
                        sort = "file_mod_time",
                        direction = "DESC"
                    },
                    scene_filter = new { }
                },
                OperationName = "FindScenes",
                Query = """
                query FindScenes($filter: FindFilterType, $scene_filter: SceneFilterType, $scene_ids: [Int!]) {
          findScenes(filter: $filter, scene_filter: $scene_filter, scene_ids: $scene_ids) {
            count
            filesize
            duration
            scenes {
              ...SlimSceneData
              __typename
            }
            __typename
          }
        }

        fragment SlimSceneData on Scene {
          id
          title
          code
          details
          urls
          date
          play_duration
          files {
            ...VideoFileData
            __typename
          }
          stash_ids {
            endpoint
            stash_id
            __typename
          }
          __typename
        }

        fragment VideoFileData on VideoFile {
          id
          path
          size
          mod_time
          duration
          width
          height
          frame_rate
          __typename
        }
        """
            };
            var response = await graphQLClient.SendQueryAsync<SceneQueryResponseType>(query);
            return response.Data.findScenes;
        }

        public async Task<Scene> UpdateScene(Scene updatedScene)
        {
            var id = updatedScene.code;

            Scene search = await ScrapeSceneId(id);
            if (search?.urls == null)
            {
                return null;
            }
            var url = search.urls.FirstOrDefault();
            Thread.Sleep(100);
            Scene scraped = await ScrapeSceneUrl(url);

            var scene = new SceneUpdateInput
            {
                id = updatedScene.id,
                title = updatedScene.title,
                code = updatedScene.code,
                details = scraped.details,
                urls = scraped.urls,
                cover_image = scraped.image,
                date = scraped.date
            };
            Scene updated = await SceneUpdate(scene);
            return updated;
        }

        public async Task<string> QueueMetadataScanJob(string path)
        {
            var input = new QueueMetadataScanJobInput()
            {
                paths = new List<string> { path }
            };


            var query = new GraphQLRequest
            {
                Variables = new
                {
                    input = input
                },
                OperationName = "MetadataScan",
                Query = """
                mutation MetadataScan($input: ScanMetadataInput!) {
                  metadataScan(input: $input)
                }
                """
            };
            var response = await graphQLClient.SendQueryAsync<QueueMetadataScanJobResponseType>(query);
            return response.Data.metadataScan;
        }

        public async Task<string> QueueMetadataIdentifyJob(string path)
        {
            var input = new QueueMetadataIdentifyJobInput()
            {
                sources = new List<QueueMetadataIdentifyJobInputScraperSource> { new QueueMetadataIdentifyJobInputScraperSource {
                        source = new QueueMetadataIdentifyJobInputScraperSource2 {
                            scraper_id = "javdb"
                        }
                    }
                },
                options = new QueueMetadataIdentifyJobInputOptions()
                {
                    fieldOptions = new List<QueueMetadataIdentifyJobInputFieldOption> { new QueueMetadataIdentifyJobInputFieldOption {
                        field = "title",
                        strategy = "MERGE",
                        createMissing = false,
                        }
                    }
                },
                paths = new List<string> { path }
            };

            var query = new GraphQLRequest
            {
                Variables = new
                {
                    input = input
                },
                OperationName = "MetadataIdentify",
                Query = """
                mutation MetadataIdentify($input: IdentifyMetadataInput!) {
                  metadataIdentify(input: $input)
                }
                """
            };
            var response = await graphQLClient.SendQueryAsync<QueueMetadataIdentifyJobResponseType>(query);
            return response.Data.metadataIdentify;
        }

        public async Task<JobStatus> FindJob(string jobId)
        {
            var query = new GraphQLRequest
            {
                Variables = new
                {
                    input = new
                    {
                        id = jobId
                    }
                },
                OperationName = "FindJob",
                Query = """
                query FindJob($input: FindJobInput!) {
                  findJob(input: $input) {
                    ...JobData
                  }
                }

                fragment JobData on Job {
                  id
                  status
                }
                """
            };
            var response = await graphQLClient.SendQueryAsync<FindJobResponseType>(query);
            return response.Data.findJob;
        }

        // helper methods

        // Returns the human-readable file size for an arbitrary, 64-bit file size 
        // The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
        private static string GetBytesReadable(long i)
        {
            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = (readable / 1024);
            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }

        private static IEnumerable<Scene> FormatJavIds(List<(string filename, string id)> scenes)
        {

            var codes = scenes
                              //.Where(s => s.filename.Contains("PMAXVR-023", StringComparison.InvariantCultureIgnoreCase))
                              .Select(s => JavIdFormatter.FormatJavId(s.filename, s.id)).Where(c => c != null);
            return codes;
        }

        public void Dispose()
        {
            graphQLClient.Dispose();
        }
    }
}
