﻿using FolderFile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SerienBenennen
{
    class ViewModel : INotifyPropertyChanged
    {
        private static ViewModel instance;

        public static ViewModel Current
        {
            get
            {
                if (instance == null) instance = new ViewModel();

                return instance;
            }
        }

        private string maxSeasonNumberText, maxEpisodeNumberText, seriesName;
        private Folder folder;
        private ObservableCollection<EpisodeName> episodesNames;

        public int MaxSeasonNumber
        {
            get
            {
                int maxSeasonNumber;

                if (!int.TryParse(maxSeasonNumberText, out maxSeasonNumber)) maxSeasonNumber = 0;

                return maxSeasonNumber;
            }
        }

        public int MaxEpisodeNumber
        {
            get
            {
                int maxEpisodeNumber;

                if (!int.TryParse(maxEpisodeNumberText, out maxEpisodeNumber)) maxEpisodeNumber = 0;

                return maxEpisodeNumber;
            }
        }

        public string MaxSeasonNumberText
        {
            get { return maxSeasonNumberText; }
            set
            {
                int maxSeasonNumber;

                if (maxSeasonNumberText == value) return;
                if (int.TryParse(value, out maxSeasonNumber)) maxSeasonNumberText = value;

                OnPropertyChanged("MaxSeasonNumberText");
            }
        }

        public string MaxEpisodeNumberText
        {
            get { return maxEpisodeNumberText; }
            set
            {
                int maxEpisodeNumber;

                if (maxEpisodeNumberText == value) return;
                if (int.TryParse(value, out maxEpisodeNumber)) maxEpisodeNumberText = value;

                OnPropertyChanged("MaxEpisodeNumberText");
            }
        }

        public string SeriesName
        {
            get { return seriesName; }
            set
            {
                if (seriesName == value) return;

                seriesName = value;
                OnPropertyChanged("SeriesName");
            }
        }

        public Folder Folder
        {
            get { return folder; }
            set
            {
                if (folder == value) return;

                folder = value;
                UpdateEpisodenNamesFromStorage();
                OnPropertyChanged("SeasonOrdner");

                SeriesName = folder.Info.Name;
            }
        }

        public ObservableCollection<EpisodeName> EpisodesNames
        {
            get { return episodesNames; }
            set
            {
                if (episodesNames == value) return;

                episodesNames = value;
                episodesNames.CollectionChanged += EpisodenNames_CollectionChanged;
                OnPropertyChanged("EpisodesNames");
            }
        }

        private ViewModel()
        {
            EpisodesNames = new ObservableCollection<EpisodeName>();
            MaxSeasonNumberText = "1";
            MaxEpisodeNumberText = "10";
        }

        private void EpisodenNames_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("EpisodesNames");
        }

        public void UpdateEpisodenNamesFromStorage()
        {
            FileInfo[] files = folder.GetFiles();
            List<EpisodeName> oldNames = episodesNames.ToList();
            ObservableCollection<EpisodeName> names = new ObservableCollection<EpisodeName>();

            foreach (FileInfo datei in files)
            {
                try
                {
                    EpisodeName episodeName = new EpisodeName(datei.FullName);

                    EpisodeName oldName = oldNames.FirstOrDefault(x => x.FullPath == episodeName.FullPath);

                    if (oldName != null) episodeName.Name = oldName.Name;

                    int insertIndex = GetInsertIndexWhenOrderedBySeasonAndEpisode(episodeName, names);
                    names.Insert(insertIndex, episodeName);
                }
                catch { }
            }

            EpisodesNames = names;
            MaxEpisodeNumberText = names.Max(x => x.EpisodeNumber).ToString();
        }

        private int GetInsertIndexWhenOrderedBySeasonAndEpisode(EpisodeName episodeName,
            ObservableCollection<EpisodeName> list)
        {
            int index = 0;

            while (index < list.Count && list[index].SeasonNumber < episodeName.SeasonNumber) index++;
            while (index < list.Count && list[index].EpisodeNumber < episodeName.EpisodeNumber) index++;

            return index;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null) return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}