using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

using psy_trance_com.DataAccessLayer.Models;

namespace psy_trance_com.Controllers
{
    public class FolderController : ApiController
    {
        List<Folder> _folders = new List<Folder>();
        List<File> _files = new List<File>();

        List<Album> _albums = new List<Album>();
        List<AlbumArtist> _albumArtists = new List<AlbumArtist>();
        List<Artist> _artists = new List<Artist>();
        List<Country>_countries = new List<Country>();
        List<Genre> _genres = new List<Genre>();
        List<Label>_labels = new List<Label>();
        List<Song> _songs = new List<Song>();

        List<Jpeg> _jpegs = new List<Jpeg>();
        List<Torrent> _torrents = new List<Torrent>();

        [HttpGet]
        public HttpResponseMessage Index(string folderName)
        {
            var httpResponseMessage = Request.CreateResponse();

            var folders = new List<Folder>
            {
                new Folder
                {
                    Name = folderName,

                    Torrents = new List<Torrent>()
                }
            };

            var torrentCreator = new MonoTorrent.Common.TorrentCreator();
            var torrentFileSource = new MonoTorrent.Common.TorrentFileSource(folderName);

            torrentCreator.Announces.Add(new List<string>
                {
                    "https://psy-trance.com/announce/"
                });

            //torrentCreator.Create(torrentFileSource, folderName + ".torrent");

            var torrents = new List<Torrent>();

            using (var memoryStream = new System.IO.MemoryStream())
            {
                torrentCreator.Create(torrentFileSource, memoryStream);
                torrents.Add(new Torrent
                {
                    Name = folderName + ".torrent",

                    Data = memoryStream.ToArray()
                });
            }

            _folders = _folders.Union(folders).ToList();
            _torrents = _torrents.Union(torrents).ToList();

            _folders.ToList().ForEach(folder =>
            {
                folder.Torrents = folder.Torrents.Union(_torrents).ToList();
            });

            var fileNames = System.IO.Directory.GetFiles(folderName, "*", System.IO.SearchOption.AllDirectories).ToList();

            fileNames.ForEach(fileName =>
            {
                using (var file = TagLib.File.Create(fileName))
                {
                    file.Tag.AlbumArtists = file.Tag.JoinedAlbumArtists.Split(';').Select(x => x.Trim()).ToArray();
                    file.Tag.Artists = file.Tag.JoinedArtists.Split(';').Select(x => x.Trim()).ToArray();
                    file.Tag.Genres = file.Tag.JoinedGenres.Split(';').Select(x => x.Trim()).ToArray();

                    file.Save();

                    var fileInfo = new System.IO.FileInfo(fileName);

                    var files = new List<File>
                    {
                        new File
                        {
                            Name = fileName,

                            BitRate = file.Properties.AudioBitrate,
                            SampleRate = file.Properties.AudioSampleRate,
                            Channels = file.Properties.AudioChannels,
                            BitsPerSample = file.Properties.BitsPerSample,

                            Size = fileInfo.Length
                        }
                    };

                    var albums = new List<Album>
                    {
                        new Album
                        {
                            Name = file.Tag.Album,

                            AlbumArtists = new List<AlbumArtist>(),
                            Artists = new List<Artist>(),
                            Genres = new List<Genre>(),
                            Songs = new List<Song>(),

                            Folder = folderName,
                            Folders = new List<Folder>(),

                            Year = (int) file.Tag.Year,

                            Jpeg = new List<Jpeg>(),
                        }
                    };

                    var albumArtists = file.Tag.AlbumArtists.Select(albumArtistName => new AlbumArtist
                    {
                        Name = albumArtistName,

                        Albums = new List<Album>(),
                        Artists = new List<Artist>(),
                        Genres = new List<Genre>(),
                        Songs = new List<Song>(),
                    }).ToList();

                    var artists = file.Tag.Artists.Select(artistName => new Artist
                    {
                        Name = artistName,

                        Albums = new List<Album>(),
                        AlbumArtists = new List<AlbumArtist>(),
                        Genres = new List<Genre>(),
                        Songs = new List<Song>(),
                    }).ToList();

                    var genres = file.Tag.Genres.Select(genreName => new Genre
                    {
                        Name = genreName,

                        Albums = new List<Album>(),
                        AlbumArtists = new List<AlbumArtist>(),
                        Artists = new List<Artist>(),
                        Songs = new List<Song>(),
                    }).ToList();

                    var songs = new List<Song>
                    {
                        new Song
                        {
                            Name = file.Tag.Title,

                            Albums = new List<Album>(),
                            AlbumArtists = new List<AlbumArtist>(),
                            Artists = new List<Artist>(),
                            Genres = new List<Genre>(),

                            File = fileName,
                            Files = new List<File>(),

                            Disc = (int) file.Tag.Disc,
                            Track = (int) file.Tag.Track,

                            Time = file.Properties.Duration
                        }
                    };

                    var jpeg = new List<Jpeg>();

                    file.Tag.Pictures.ToList().ForEach(picture =>
                    {
                        using (var memoryStream = new System.IO.MemoryStream(picture.Data.Data))
                        {
                            jpeg.Add(new Jpeg
                            {
                                Name = folderName + ".jpeg",

                                Data = memoryStream.ToArray()
                            });
                        }
                    });

                    _albums = _albums.Union(albums).ToList();
                    _albumArtists = _albumArtists.Union(albumArtists).ToList();
                    _artists = _artists.Union(artists).ToList();
                    _genres = _genres.Union(genres).ToList();
                    _songs = _songs.Union(songs).ToList();

                    _files = _files.Union(files).ToList();

                    _jpegs = _jpegs.Union(jpeg).ToList();

                    _artists.Intersect(artists).ToList().ForEach(artist =>
                    {
                        artist.Songs = artist.Songs.Union(_songs.Intersect(songs)).ToList();
                    });

                    _songs.Intersect(songs).ToList().ForEach(song =>
                    {
                        song.Artists = song.Artists.Union(_artists.Intersect(artists)).ToList();

                        song.Files = song.Files.Union(_files.Intersect(files)).ToList();
                    });
                }
            });

            _folders.ToList().ForEach(folder =>
            {
                folder.BitRate =  (int) _files.Select(x => x.BitRate).Average();
                folder.BitsPerSample = (int)_files.Select(x => x.BitsPerSample).Average();
                folder.Channels = (int)_files.Select(x => x.Channels).Average();
                folder.SampleRate = (int)_files.Select(x => x.SampleRate).Average();

                folder.Size = _files.Select(x => x.Size).Sum();
            });

            _albums.ToList().ForEach(album =>
            {
                album.AlbumArtists = album.AlbumArtists.Union(_albumArtists).ToList();
                album.Artists = album.Artists.Union(_artists).ToList();
                album.Genres = album.Genres.Union(_genres).ToList();
                album.Songs = album.Songs.Union(_songs).ToList();

                album.Folders = album.Folders.Union(_folders).ToList();

                album.Jpeg = album.Jpeg.Union(_jpegs).ToList();
            });

            _albumArtists.ToList().ForEach(albumArtist =>
            {
                albumArtist.Albums = albumArtist.Albums.Union(_albums).ToList();
                albumArtist.Artists = albumArtist.Artists.Union(_artists).ToList();
                albumArtist.Genres = albumArtist.Genres.Union(_genres).ToList();
                albumArtist.Songs = albumArtist.Songs.Union(_songs).ToList();
            });

            _artists.ToList().ForEach(artist =>
            {
                artist.Albums = artist.Albums.Union(_albums).ToList();
                artist.AlbumArtists = artist.AlbumArtists.Union(_albumArtists).ToList();
                artist.Genres = artist.Genres.Union(_genres).ToList();
                //artist.Songs = artist.Songs.Union(_songs).ToList();
            });

            _genres.ToList().ForEach(genre =>
            {
                genre.Albums = genre.Albums.Union(_albums).ToList();
                genre.AlbumArtists = genre.AlbumArtists.Union(_albumArtists).ToList();
                genre.Artists = genre.Artists.Union(_artists).ToList();
                genre.Songs = genre.Songs.Union(_songs).ToList();
            });

            _songs.ToList().ForEach(song =>
            {
                song.Albums = song.Albums.Union(_albums).ToList();
                song.AlbumArtists = song.AlbumArtists.Union(_albumArtists).ToList();
                //song.Artists = song.Artists.Union(_artists).ToList();
                song.Genres = song.Genres.Union(_genres).ToList();

                //song.Files = song.Files.Union(_files).ToList();
            });

            return httpResponseMessage;
        }
    }
}