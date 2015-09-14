using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web.Http;

using psy_trance_com.DataAccessLayer;
using psy_trance_com.DataAccessLayer.Models;

namespace psy_trance_com.Controllers
{
    public class FolderController : ApiController
    {
        [HttpGet]
        public void Index()
        {
            var folderNames = Directory.GetDirectories(@"C:\mp3", "*", SearchOption.AllDirectories).ToList();

            folderNames.ForEach(folderName =>
            {
                using (var dbContext = new DbContext())
                {
                    var fileNames = Directory.GetFiles(folderName).Where(x => x.EndsWith(".flac") || x.EndsWith(".mp3")).ToList();

                    if (fileNames.Count > 0)
                    {
                        var torrentCreator = new MonoTorrent.Common.TorrentCreator();
                        var torrentFileSource = new MonoTorrent.Common.TorrentFileSource(folderName);

                        torrentCreator.Announces.Add(new List<string>
                        {
                            "https://psy-trance.com/announce/"
                        });

                        var torrent = new List<Torrent>();

                        using (var memoryStream = new MemoryStream())
                        {
                            torrentCreator.Create(torrentFileSource, memoryStream);

                            torrent.Add(dbContext.Torrent.FirstOrDefault(x => x.Name == folderName) ??
                                        new Torrent
                                        {
                                            Name = folderName,

                                            Data = memoryStream.ToArray()
                                        });
                        }

                        fileNames.ForEach(fileName =>
                        {
                            using (var file = TagLib.File.Create(fileName))
                            {
                                file.Tag.AlbumArtists = file.Tag.JoinedAlbumArtists.Split(';').Select(x => x.Trim()).ToArray();
                                file.Tag.Artists = file.Tag.JoinedArtists.Split(';').Select(x => x.Trim()).ToArray();
                                file.Tag.Genres = file.Tag.JoinedGenres.Split(';').Select(x => x.Trim()).ToArray();

                                file.Save();

                                var albums = new List<Album>();

                                albums.Add(dbContext.Albums.FirstOrDefault(x => x.Folder == folderName) ??
                                           new Album
                                           {
                                               Name = file.Tag.Album,

                                               AlbumArtists = new List<AlbumArtist>(),
                                               Artists = new List<Artist>(),
                                               Countries = new List<Country>(),
                                               Genres = new List<Genre>(),
                                               Labels = new List<Label>(),
                                               Songs = new List<Song>(),

                                               Folder = folderName,
                                               Folders = new List<Folder>(),

                                               Jpeg = new List<Jpeg>(),
                                               Torrent = new List<Torrent>()
                                           });

                                var albumArtists = new List<AlbumArtist>();

                                file.Tag.AlbumArtists.ToList().ForEach(albumArtistName =>
                                {
                                    albumArtists.Add(dbContext.AlbumArtists.FirstOrDefault(x => x.Name == albumArtistName) ??
                                                     new AlbumArtist
                                                     {
                                                         Name = albumArtistName,

                                                         Albums = new List<Album>(),
                                                         Artists = new List<Artist>(),
                                                         Countries = new List<Country>(),
                                                         Genres = new List<Genre>(),
                                                         Labels = new List<Label>(),
                                                         Songs = new List<Song>(),
                                                     });
                                });

                                var artists = new List<Artist>();

                                file.Tag.Artists.ToList().ForEach(artistName =>
                                {
                                    artists.Add(dbContext.Artists.FirstOrDefault(x => x.Name == artistName) ??
                                                new Artist
                                                {
                                                    Name = artistName,

                                                    Albums = new List<Album>(),
                                                    AlbumArtists = new List<AlbumArtist>(),
                                                    Countries = new List<Country>(),
                                                    Genres = new List<Genre>(),
                                                    Labels = new List<Label>(),
                                                    Songs = new List<Song>(),
                                                });
                                });

                                var countries = new List<Country>();

                                var genres = new List<Genre>();

                                file.Tag.Genres.ToList().ForEach(genreName =>
                                {
                                    genres.Add(dbContext.Genres.FirstOrDefault(x => x.Name == genreName) ??
                                               new Genre
                                               {
                                                   Name = genreName,

                                                   Albums = new List<Album>(),
                                                   AlbumArtists = new List<AlbumArtist>(),
                                                   Artists = new List<Artist>(),
                                                   Countries = new List<Country>(),
                                                   Labels = new List<Label>(),
                                                   Songs = new List<Song>(),
                                               });
                                });

                                var labels = new List<Label>();

                                var songs = new List<Song>();

                                songs.Add(dbContext.Songs.FirstOrDefault(x => x.File == fileName) ??
                                          new Song
                                          {
                                              Name = file.Tag.Title,

                                              Albums = new List<Album>(),
                                              AlbumArtists = new List<AlbumArtist>(),
                                              Artists = new List<Artist>(),
                                              Countries = new List<Country>(),
                                              Genres = new List<Genre>(),
                                              Labels = new List<Label>(),

                                              File = fileName
                                          });

                                var jpeg = new List<Jpeg>();

                                file.Tag.Pictures.ToList().ForEach(picture =>
                                {
                                    using (var memoryStream = new MemoryStream(picture.Data.Data))
                                    {
                                        jpeg.Add(dbContext.Jpeg.FirstOrDefault(x => x.Name == folderName) ??
                                                 new Jpeg
                                                 {
                                                     Name = folderName,

                                                     Data = memoryStream.ToArray()
                                                 });
                                    }
                                });

                                albums.ForEach(album =>
                                {
                                    album.AlbumArtists = album.AlbumArtists.Union(albumArtists).ToList();
                                    album.Artists = album.Artists.Union(artists).ToList();
                                    album.Countries = album.Countries.Union(countries).ToList();
                                    album.Genres = album.Genres.Union(genres).ToList();
                                    album.Labels = album.Labels.Union(labels).ToList();
                                    album.Songs = album.Songs.Union(songs).ToList();

                                    album.Jpeg = album.Jpeg.Union(jpeg).ToList();
                                    album.Torrent = album.Torrent.Union(torrent).ToList();

                                    dbContext.Albums.AddOrUpdate(album);
                                });

                                albumArtists.ForEach(albumArtist =>
                                {
                                    albumArtist.Albums = albumArtist.Albums.Union(albums).ToList();
                                    albumArtist.Artists = albumArtist.Artists.Union(artists).ToList();
                                    albumArtist.Countries = albumArtist.Countries.Union(countries).ToList();
                                    albumArtist.Genres = albumArtist.Genres.Union(genres).ToList();
                                    albumArtist.Labels = albumArtist.Labels.Union(labels).ToList();
                                    albumArtist.Songs = albumArtist.Songs.Union(songs).ToList();

                                    dbContext.AlbumArtists.AddOrUpdate(albumArtist);
                                });

                                artists.ForEach(artist =>
                                {
                                    artist.Albums = artist.Albums.Union(albums).ToList();
                                    artist.AlbumArtists = artist.AlbumArtists.Union(albumArtists).ToList();
                                    artist.Countries = artist.Countries.Union(countries).ToList();
                                    artist.Genres = artist.Genres.Union(genres).ToList();
                                    artist.Labels = artist.Labels.Union(labels).ToList();
                                    artist.Songs = artist.Songs.Union(songs).ToList();

                                    dbContext.Artists.AddOrUpdate(artist);
                                });

                                countries.ForEach(country =>
                                {
                                    country.Albums = country.Albums.Union(albums).ToList();
                                    country.AlbumArtists = country.AlbumArtists.Union(albumArtists).ToList();
                                    country.Artists = country.Artists.Union(artists).ToList();
                                    country.Genres = country.Genres.Union(genres).ToList();
                                    country.Labels = country.Labels.Union(labels).ToList();
                                    country.Songs = country.Songs.Union(songs).ToList();

                                    dbContext.Countries.AddOrUpdate(country);
                                });

                                genres.ForEach(genre =>
                                {
                                    genre.Albums = genre.Albums.Union(albums).ToList();
                                    genre.AlbumArtists = genre.AlbumArtists.Union(albumArtists).ToList();
                                    genre.Artists = genre.Artists.Union(artists).ToList();
                                    genre.Countries = genre.Countries.Union(countries).ToList();
                                    genre.Labels = genre.Labels.Union(labels).ToList();
                                    genre.Songs = genre.Songs.Union(songs).ToList();

                                    dbContext.Genres.AddOrUpdate(genre);
                                });

                                labels.ForEach(label =>
                                {
                                    label.Albums = label.Albums.Union(albums).ToList();
                                    label.AlbumArtists = label.AlbumArtists.Union(albumArtists).ToList();
                                    label.Artists = label.Artists.Union(artists).ToList();
                                    label.Countries = label.Countries.Union(countries).ToList();
                                    label.Genres = label.Genres.Union(genres).ToList();
                                    label.Songs = label.Songs.Union(songs).ToList();

                                    dbContext.Labels.AddOrUpdate(label);
                                });

                                songs.ForEach(song =>
                                {
                                    song.Albums = song.Albums.Union(albums).ToList();
                                    song.AlbumArtists = song.AlbumArtists.Union(albumArtists).ToList();
                                    song.Artists = song.Artists.Union(artists).ToList();
                                    song.Countries = song.Countries.Union(countries).ToList();
                                    song.Genres = song.Genres.Union(genres).ToList();
                                    song.Labels = song.Labels.Union(labels).ToList();

                                    dbContext.Songs.AddOrUpdate(song);
                                });

                                dbContext.SaveChanges();
                            }
                        });
                    }
                }
            });
        }
    }
}