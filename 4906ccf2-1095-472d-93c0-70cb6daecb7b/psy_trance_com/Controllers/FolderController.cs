using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

using psy_trance_com.DataAccessLayer.Models;

namespace psy_trance_com.Controllers
{
    public class FolderController : ApiController
    {
        List<Album> _albums = new List<Album>();
        List<AlbumArtist> _albumArtists = new List<AlbumArtist>();
        List<Artist> _artists = new List<Artist>();
        List<Genre> _genres = new List<Genre>();
        List<Song> _songs = new List<Song>();

        List<Folder> _folders = new List<Folder>();
        List<File> _files = new List<File>();

        List<Jpeg> _jpeg = new List<Jpeg>();

        [HttpGet]
        public HttpResponseMessage Index(string folderName)
        {
            var httpResponseMessage = Request.CreateResponse();

            var torrentCreator = new MonoTorrent.Common.TorrentCreator();
            var torrentFileSource = new MonoTorrent.Common.TorrentFileSource(folderName);

            torrentCreator.Announces.Add(new List<string>
                {
                    "https://psy-trance.com/announce/"
                });

            torrentCreator.Create(torrentFileSource, folderName + ".torrent");

            var torrent = new Torrent();

            using (var memoryStream = new System.IO.MemoryStream())
            {
                torrentCreator.Create(torrentFileSource, memoryStream);

                torrent.Data = memoryStream.ToArray();
            }

            var fileNames = System.IO.Directory.GetFiles(folderName, "*", System.IO.SearchOption.AllDirectories).ToList();

            fileNames.ForEach(fileName =>
            {
                using (var file = TagLib.File.Create(fileName))
                {
                    file.Tag.AlbumArtists = file.Tag.JoinedAlbumArtists.Split(';').Select(x => x.Trim()).ToArray();
                    file.Tag.Artists = file.Tag.JoinedArtists.Split(';').Select(x => x.Trim()).ToArray();
                    file.Tag.Genres = file.Tag.JoinedGenres.Split(';').Select(x => x.Trim()).ToArray();

                    file.Save();

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

                            Year = (int) file.Tag.Year
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

                    var folders = new List<Folder>
                    {
                        new Folder
                        {
                            Name = folderName
                        }
                    };

                    var files = new List<File>
                    {
                        new File
                        {
                            Name = fileName
                        }
                    };

                    var jpeg = new List<Jpeg>();

                    file.Tag.Pictures.ToList().ForEach(picture =>
                    {
                        using (var memoryStream = new System.IO.MemoryStream(picture.Data.Data))
                        {
                            jpeg.Add(new Jpeg
                            {
                                Data = memoryStream.ToArray()
                            });
                        }
                    });

                    _albums = _albums.Union(albums).ToList();
                    _albumArtists = _albumArtists.Union(albumArtists).ToList();
                    _artists = _artists.Union(artists).ToList();
                    _genres = _genres.Union(genres).ToList();
                    _songs = _songs.Union(songs).ToList();

                    _folders = _folders.Union(folders).ToList();
                    _files = _files.Union(files).ToList();

                    _jpeg = _jpeg.Union(jpeg).ToList();

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

            _albums.ToList().ForEach(album =>
            {
                album.AlbumArtists = album.AlbumArtists.Union(_albumArtists).ToList();
                album.Artists = album.Artists.Union(_artists).ToList();
                album.Genres = album.Genres.Union(_genres).ToList();
                album.Songs = album.Songs.Union(_songs).ToList();

                album.Folders = album.Folders.Union(_folders).ToList();

                album.Jpeg = album.Jpeg.Union(_jpeg).ToList();
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