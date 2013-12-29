using Id3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace ID3PSModule
{
    [Cmdlet(VerbsCommon.Get, "ID3")]
    public class GetID3DataCmdLet : PSCmdlet
    {

        [Parameter(Mandatory = true, Position = 0)]
        public string FilePath { get; set; }



        private bool IsMP3(string filePath)
        {
            var ext = Path.GetExtension(filePath);
            return string.Compare(ext, ".mp3", true) == 0;
        }

        protected override void ProcessRecord()
        {
            if (!IsMP3(FilePath))
            {
                WriteObject(null);
                return;
            }

            try
            {
                var mp3file = new Mp3File(FilePath, Mp3Permissions.Read);
                using (mp3file)
                {
                    var tag = mp3file.GetTag(Id3TagFamily.FileStartTag);
                    if (tag == null)
                    {
                        WriteObject(null);
                        return;
                    }
                    var result = new Id3TagInfo()
                    {
                        Album = tag.Album.Value,
                        Artists = tag.Artists.Value,
                        Title = tag.Title
                    };
                    WriteObject(result);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "MP3_001", ErrorCategory.InvalidArgument, FilePath));
                WriteObject(null);
            }
        }
    }
}
