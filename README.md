* build the image  
docker build . -t update_stash_api

* run the image  
docker run -p 8080:8080 -e ConnectionStrings:stashurl='http://stash_ip:port/graphql' -e ConnectionStrings:apikey='stash apikey' update_stash_api

* how to notify/trigger/hook the api  
curl -X GET 'http://stash_ip:port/api/UpdateStash?name=whatever&path=/path/to/scene/folder'

* /path/to/scene/folder caution  
fix this line according to how you map the actual file, i mapped my folder downloads to '/downloads' in my qbittorrent service and to '/data/torr' in my stash service  
[var stashPath = path.Replace("/downloads", "/data/torr");](https://github.com/Kahdeg-15520487/update_stash_api/blob/d8bcda3b038caf8131096719d3d768fbea446b89/update_stash_api/Controllers/UpdateStashController.cs#L42C1-L42C74)
