docker build . -t update_stash_api

docker run -p 8080:8080 -e ConnectionStrings:stashurl='http://ip:port/graphql' -e ConnectionStrings:apikey='stash apikey' update_stash_api
