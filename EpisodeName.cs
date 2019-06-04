using System;
using System.ComponentModel;
using System.IO;

namespace SerienBenennen
{
    public class EpisodeName : INotifyPropertyChanged
    {
        private string name;

        public int EpisodeNumber { get; private set; }

        public int SeasonNumber { get; private set; }

        public string PathAndName { get { return ToString(); } }

        public FileInfo File { get; private set; }

        public string FullPath { get { return File.FullName; } }

        public string FileNameWithoutExtension { get { return File.Name.Remove(File.Name.Length - File.Extension.Length); } }

        public string DirectoryName { get { return File.DirectoryName; } }

        public string Extension { get { return File.Extension; } }

        public string Name
        {
            get { return name; }
            set
            {
                if (name == value) return;

                name = value;
                OnPropertyChanged("PathAndName");
            }
        }

        public EpisodeName(string path)
        {
            File = new FileInfo(path);

            if (!(HaveSetWithFinalPattern() || HaveSetWithCodePattern())) throw new Exception("Not Changeable Episode");
        }

        public override string ToString()
        {
            return FileNameWithoutExtension + " : " + Name;
        }

        public void Save()
        {
            if (Name != null)
            {
                foreach (char invalidFileNameChar in Path.GetInvalidFileNameChars())
                {
                    Name = Name.Replace(invalidFileNameChar.ToString(), "");
                }
            }

            string finalFileName = string.Format("{0} S{1}E{2} - {3}{4}",
                ViewModel.Current.SeriesName, SeasonNumber, GetEpisodenNumberWithZeros(), Name, Extension);

            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                finalFileName = finalFileName.Replace(invalidChar.ToString(), "");
            }

            string finalPath = Path.Combine(DirectoryName, finalFileName);

            try
            {
                File.MoveTo(finalPath);
            }
            catch { }
        }

        private string GetEpisodenNumberWithZeros()
        {
            int maxNumberLength = ViewModel.Current.MaxEpisodeNumber.ToString().Length;
            string numberText = EpisodeNumber.ToString();

            for (int i = numberText.Length; i < maxNumberLength; i++)
            {
                numberText = "0" + numberText;
            }

            return numberText;
        }

        private bool HaveSetWithCodePattern()
        {
            int codeIndex = GetEpisodeCodeIndex();

            if (codeIndex == -1) return false;

            int codeCount = GetEpisodeCodeCount(codeIndex);

            SetEpisodenNumberWithCode(codeIndex, codeCount);
            SetSeasonNumberWithCode(codeIndex, codeCount);
            SetNameFromCodePattern(codeIndex + codeCount);

            return true;
        }

        private int GetEpisodeCodeIndex()
        {
            for (int i = 0; i + 2 < FileNameWithoutExtension.Length; i++)
            {
                if (char.IsNumber(FileNameWithoutExtension[i]) &&
                    char.IsNumber(FileNameWithoutExtension[i + 1]) &&
                    char.IsNumber(FileNameWithoutExtension[i + 2])) return i;
            }

            return -1;
        }

        private int GetEpisodeCodeCount(int codeIndex)
        {
            int codeCount = 0;

            while (codeIndex + codeCount < FileNameWithoutExtension.Length &&
                char.IsNumber(FileNameWithoutExtension[codeIndex + codeCount])) codeCount++;

            return codeCount;
        }

        private void SetEpisodenNumberWithCode(int codeIndex, int codeCount)
        {
            int episodeIndex, episodeCount;

            episodeIndex = GetEpisodeNumberCodeIndex(codeIndex, codeCount);
            episodeCount = GetEpisodeNumberCodeCount(codeCount);

            EpisodeNumber = GetNumber(episodeIndex, episodeCount);
        }

        private int GetEpisodeNumberCodeCount(int codeCount)
        {
            return codeCount - GetSeasonNumberCodeCount(codeCount);
        }

        private int GetEpisodeNumberCodeIndex(int codeIndex, int codeCount)
        {
            return codeIndex + codeCount - GetEpisodeNumberCodeCount(codeCount);
        }

        private void SetSeasonNumberWithCode(int codeIndex, int codeCount)
        {
            int seasonIndex, seasonCount;

            seasonIndex = codeIndex;
            seasonCount = GetSeasonNumberCodeCount(codeCount);

            SeasonNumber = GetNumber(seasonIndex, seasonCount);
        }

        private int GetSeasonNumberCodeCount(int codeCount)
        {
            return ViewModel.Current.MaxSeasonNumber;
        }

        private void SetNameFromCodePattern(int nameStartIndex)
        {
            if (nameStartIndex >= FileNameWithoutExtension.Length) return;

            Name = FileNameWithoutExtension.Remove(0, nameStartIndex).Trim();
        }

        private bool HaveSetWithFinalPattern()
        {
            int seasonIndex, seasonCount, episodeIndex, episodeCount;

            if (!GetFinalPatternIndexesAndCounts(out seasonIndex, out seasonCount,
                out episodeIndex, out episodeCount)) return false;

            EpisodeNumber = GetNumber(episodeIndex, episodeCount);
            SeasonNumber = GetNumber(seasonIndex, seasonCount);

            SetNameFromFinalPattern(episodeIndex + episodeCount);

            return true;
        }

        private bool GetFinalPatternIndexesAndCounts(out int seasonIndex,
            out int seasonCount, out int episodeIndex, out int episodeCount)
        {
            string fileNameWE = FileNameWithoutExtension.ToLower();
            seasonIndex = 0;

            do
            {
                episodeIndex = -1;
                seasonCount = episodeCount = 0;

                while (seasonIndex < fileNameWE.Length && fileNameWE[seasonIndex] != 's') seasonIndex++;

                seasonIndex++;

                while (seasonIndex + seasonCount < fileNameWE.Length &&
                    char.IsNumber(fileNameWE[seasonIndex + seasonCount])) seasonCount++;

                if (seasonIndex + seasonCount < fileNameWE.Length && fileNameWE[seasonIndex + seasonCount] == 'e')
                {
                    episodeIndex = seasonIndex + seasonCount + 1;

                    while (episodeIndex + episodeCount < fileNameWE.Length &&
                        char.IsNumber(fileNameWE[episodeIndex + episodeCount])) episodeCount++;

                    if (seasonIndex != -1 && seasonCount != 0 && episodeIndex != -1 && episodeCount != 0) return true;
                }

                seasonIndex++;
            }
            while (seasonIndex < fileNameWE.Length);

            return false;
        }

        private int GetNumber(int index, int count)
        {
            string numberText = "";

            for (int i = 0; i < count; i++) numberText += FileNameWithoutExtension[index + i];

            return int.Parse(numberText);
        }

        private void SetNameFromFinalPattern(int startIndex)
        {
            string fileNameWE = FileNameWithoutExtension.ToLower();

            while (startIndex < fileNameWE.Length && fileNameWE[startIndex] == ' ') startIndex++;
            while (startIndex < fileNameWE.Length && fileNameWE[startIndex] == '-') startIndex++;
            while (startIndex < fileNameWE.Length && fileNameWE[startIndex] == ' ') startIndex++;

            if (startIndex >= fileNameWE.Length) return;

            Name = FileNameWithoutExtension.Remove(0, startIndex).TrimStart(' ').TrimEnd(' ');
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null) return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
