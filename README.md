# Deserializer

[![Build status](https://ci.appveyor.com/api/projects/status/n0xcqvxt4ci6h7vd?svg=true)](https://ci.appveyor.com/project/merdacz/deserializer)

## Summary
Deserializes JS objects of given shape using RestSharp JsonDeserializer underneath. Given library limitations as described on [stack overflow](http://stackoverflow.com/a/29217883) the process happens two-phase. First is deserialization into interim representation using `JsonArray` for the outer `Entries` list. Thereafter individual entries from that array are being individually re-processed, which includes serialization and final deserialization. The logic is wrapped into `DocumentDeserializer` class and its usage is demonstrated in unit test `DocumentDeserializerTests#Original_structure_indirect_deserialization`.

## Load tests
Since above mentioned re-processing happens to achieve fully-typed result class it obviously has performance impact. `LoadTests` 'unit-tests' serve the purpose of meassuring that against two variables - number of entries and number of runs. `OriginalLoadTest` uses the two-phase parsing as described above and `AlternativeLoadTest` depends strictly on `JsonDeserializer` by using a bit modified input JSON structure. The results are as follows (sorted by ascending entries multiplied loops):

| Version       | # of entries  | # of loops  | Total time [ms]
| ------------- |:-------------:|:-----------:| ---------------
| Original      | 2             | 500         | 147  
| Alternative   | 2             | 500         | 119
| Original      | 10            | 500         | 505 *
| Alternative   | 10            | 500         | 897 *
| Original      | 2             | 5000        | 1548
| Alternative   | 2             | 5000        | 1192
| Original      | 20            | 500         | 919
| Alternative   | 20            | 500         | 718
| Original      | 10            | 5000        | 4906
| Alternative   | 10            | 5000        | 3846
| Original      | 20            | 5000        | 9171
| Alternative   | 20            | 5000        | 7348

Most of the cases alternative is superior, however for 10/500 case it is the opposite. Since the result persisted as such on subsequent [1.0.7](https://ci.appveyor.com/project/merdacz/deserializer/build/1.0.7)
and [1.0.8](https://ci.appveyor.com/project/merdacz/deserializer/build/1.0.8) runs the guess is that GC collection may be influencing
the result. This requires further investigation.

### Original format
```json
{
  "has_title": true,
  "title": "GoodLuck",
  "entries": [
    [
      "/gettingstarted.pdf",
      {
        "thumb_exists": false,
        "path": "/GettingStarted.pdf",
        "client_mtime": "Wed, 08 Jan 2014 18:00:54+0000",
        "bytes": 249159
      }
    ],
    [
      "/task.jpg",
      {
        "thumb_exists": true,
        "path": "/Task.jpg",
        "client_mtime": "Tue, 14 Jan 2014 05:53:57+0000",
        "bytes": 207696
      }
    ]
  ]
}
```
### Alternative format
```json
{
  "has_title": true,
  "title": "GoodLuck",
  "entries": {
    "/gettingstarted.pdf": {
      "thumb_exists": false,
      "path": "/GettingStarted.pdf",
      "client_mtime": "Wed, 08 Jan 2014 18:00:54+0000",
      "bytes": 249159
    },
    "/task.jpg": {
      "thumb_exists": true,
      "path": "/Task.jpg",
      "client_mtime": "Tue, 14 Jan 2014 05:53:57+0000",
      "bytes": 207696
    }
  }
}
```
