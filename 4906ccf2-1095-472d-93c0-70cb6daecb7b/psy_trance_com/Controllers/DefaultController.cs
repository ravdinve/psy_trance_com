﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

using psy_trance_com.DataAccessLayer.Models;

namespace psy_trance_com.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Index()
        {
            var httpResponseMessage = Request.CreateResponse();

            var folders = System.IO.Directory.GetDirectories(@"C:\mp3", "*", System.IO.SearchOption.AllDirectories)
                .ToList();

            folders.ForEach(folder =>
            {
                var files = System.IO.Directory.GetFiles(folder, "*.mp3", System.IO.SearchOption.TopDirectoryOnly)
                    .ToList();

                var albums = new List<Album>();
                var albumArtists = new List<AlbumArtist>();
                var artists = new List<Artist>();
                var genres = new List<Genre>();
                var songs = new List<Song>();

                files.ForEach(file =>
                {
                    using (var tagLib = TagLib.File.Create(file))
                    {
                        tagLib.Tag.AlbumArtists = tagLib.Tag.JoinedAlbumArtists.Split(';').Select(x => x.Trim()).ToArray();
                        tagLib.Tag.Artists = tagLib.Tag.JoinedArtists.Split(';').Select(x => x.Trim()).ToArray();
                        tagLib.Tag.Genres = tagLib.Tag.JoinedGenres.Split(';').Select(x => x.Trim()).ToArray();

                        tagLib.Save();

                        var _albums = new List<Album>
                        {
                            new Album
                            {
                                Name = tagLib.Tag.Album,

                                AlbumArtists = new List<AlbumArtist>(),
                                Artists = new List<Artist>(),
                                Genres = new List<Genre>(),
                                Songs = new List<Song>(),

                                Folder = folder
                            }
                        };

                        albums = albums.Union(_albums).ToList();

                        var _albumArtists = tagLib.Tag.AlbumArtists.Select(name => new AlbumArtist
                        {
                            Name = name,

                            Albums = new List<Album>(),
                            Artists = new List<Artist>(),
                            Genres = new List<Genre>(),
                            Songs = new List<Song>(),
                        });

                        albumArtists = albumArtists.Union(_albumArtists).ToList();

                        var _artists = tagLib.Tag.Artists.Select(name => new Artist
                        {
                            Name = name,

                            Albums = new List<Album>(),
                            AlbumArtists = new List<AlbumArtist>(),
                            Genres = new List<Genre>(),
                            //Songs = new List<Song>(),
                        });

                        artists = artists.Union(_artists).ToList();

                        var _genres = tagLib.Tag.Genres.Select(name => new Genre
                        {
                            Name = name,

                            Albums = new List<Album>(),
                            AlbumArtists = new List<AlbumArtist>(),
                            Artists = new List<Artist>(),
                            Songs = new List<Song>(),
                        });

                        genres = genres.Union(_genres).ToList();

                        var _songs = new List<Song>
                        {
                            new Song
                            {
                                Name = tagLib.Tag.Title,

                                Albums = new List<Album>(),
                                AlbumArtists = new List<AlbumArtist>(),
                                //Artists = new List<Artist>(),
                                Genres = new List<Genre>(),

                                File = file
                            }
                        };

                        songs = songs.Union(_songs).ToList();

                        artists.Intersect(_artists).ToList().ForEach(artist =>
                        {
                            artist.Songs = songs.Intersect(_songs).ToList();
                        });

                        songs.Intersect(_songs).ToList().ForEach(song =>
                        {
                            song.Artists = artists.Intersect(_artists).ToList();
                        });
                    }
                });

                albums.ToList().ForEach(album =>
                {
                    album.AlbumArtists = album.AlbumArtists.Union(albumArtists).ToList();
                    album.Artists = album.Artists.Union(artists).ToList();
                    album.Genres = album.Genres.Union(genres).ToList();
                    album.Songs = album.Songs.Union(songs).ToList();
                });

                albumArtists.ToList().ForEach(albumArtist =>
                {
                    albumArtist.Albums = albumArtist.Albums.Union(albums).ToList();
                    albumArtist.Artists = albumArtist.Artists.Union(artists).ToList();
                    albumArtist.Genres = albumArtist.Genres.Union(genres).ToList();
                    albumArtist.Songs = albumArtist.Songs.Union(songs).ToList();
                });

                artists.ToList().ForEach(artist =>
                {
                    artist.Albums = artist.Albums.Union(albums).ToList();
                    artist.AlbumArtists = artist.AlbumArtists.Union(albumArtists).ToList();
                    artist.Genres = artist.Genres.Union(genres).ToList();
                    //artist.Songs = artist.Songs.Union(songs).ToList();
                });

                genres.ToList().ForEach(genre =>
                {
                    genre.Albums = genre.Albums.Union(albums).ToList();
                    genre.AlbumArtists = genre.AlbumArtists.Union(albumArtists).ToList();
                    genre.Artists = genre.Artists.Union(artists).ToList();
                    genre.Songs = genre.Songs.Union(songs).ToList();
                });

                songs.ToList().ForEach(song =>
                {
                    song.Albums = song.Albums.Union(albums).ToList();
                    song.AlbumArtists = song.AlbumArtists.Union(albumArtists).ToList();
                    //song.Artists = song.Artists.Union(artists).ToList();
                    song.Genres = song.Genres.Union(genres).ToList();
                });
            });

            return httpResponseMessage;
        }
    }
}