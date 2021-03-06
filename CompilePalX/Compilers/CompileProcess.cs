﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using CompilePalX.Compiling;

namespace CompilePalX
{
    class CompileProcess
    {
        public CompileProcess(string metadataFile)
        {
            var lines = File.ReadAllLines(metadataFile);

            Name = lines[0];
            Path = GameConfigurationManager.SubstituteValues(lines[1]);
            ParameterFile = lines[2];
            baseParameters = lines[3];
            Order = float.Parse(lines[4], CultureInfo.InvariantCulture);
            DoRun = bool.Parse(lines[5]);
            ReadOutput = bool.Parse(lines[6]);

            CompilePalLogger.LogLine("Loaded {0} from {1} with {2} at order {3}", Name, metadataFile, ParameterFile, Order);

            ParameterList = ConfigurationManager.GetParameters(ParameterFile);

            MetadataFile = metadataFile;

        }

        public string Name;
        public string Path;
        public float Order { get; set; }
        public string ParameterFile;
        public string MetadataFile;
        public bool DoRun;
        public bool ReadOutput;


        public Process Process;

        public string PresetFile
        {
            get { return System.IO.Path.ChangeExtension(ParameterFile, "csv"); }
        }

        public virtual void Run(CompileContext context)
        {
            
        }

        private string baseParameters;

        public ObservableCollection<ConfigItem> ParameterList = new ObservableCollection<ConfigItem>();
        public ObservableDictionary<string, ObservableCollection<ConfigItem>> PresetDictionary = new ObservableDictionary<string, ObservableCollection<ConfigItem>>();


        public string GetParameterString()
        {
            string parameters = string.Empty;
            foreach (var parameter in PresetDictionary[ConfigurationManager.CurrentPreset])
            {
                parameters += parameter.Parameter;
                if (parameter.CanHaveValue && !string.IsNullOrEmpty(parameter.Value))
                    parameters += " " + parameter.Value;
            }

            parameters += baseParameters;

            return parameters;
        }

        public void SaveMetadata()
        {
            var lines = File.ReadAllLines(MetadataFile);

            lines[5] = DoRun.ToString();

            File.WriteAllLines(MetadataFile, lines);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    class CompileContext
    {
        public string MapFile;
        public GameConfiguration Configuration;
        public string BSPFile;
        public string CopyLocation;
    }
}
