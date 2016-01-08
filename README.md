# JB.Id3Editor


## What
'JB.Id3Editor' is a cross-platform command line tool that provides [ID3 tag](http://id3.org/) modifications for one or multiple files, the later optionally in a high-performance, parallel way.

## Why
I a personal itch to scratch and because the area of existing [ID3 tag](http://id3.org/) [manipulation tools out there](http://lmgtfy.com/?q=id3+tag+editor) is basically a minefield of overpriced ad- & malware while at the same time crash-alicious and user-experience wise abominations of tools, I decided to write my own.
This one performs only a limited set of functionalities but it does it well, fast and reliable.

### Why (continued)
My specific use-case grew out of a frustration with my car's entertainment system - the [Audi MMI (Multi Media Interface)](https://en.wikipedia.org/wiki/Multi_Media_Interface) has a tendency to fallback to a visually questionable (IMHO) set of cover art in case your file(s) do a) not come with an embedded cover art and b) provide a Genre ID3 tag.
Audi, while making really nice cars (again - IMHO), chose default cover art that looks, to me, visually displeasing up to disturbing (*who wants to see heavly sweating, half-naked men?*).

Your mileage may vary or your use-case may be different, but this tool right here allows, among other things, to write a calm, neutral default cover art to file(s) and thereby overriding Audi MMI's choice. All good again.

## Disclaimer
**As the [License](https://github.com/jbattermann/JB.Id3Editor/blob/master/LICENSE) states, *this tool is provided without warranty of any kind*. USE AT YOUR OWN RISK. This may or may not damage and destroy everything, leave your computer and file(s) unusable.
It should not, but it may. If you encounter any problems, you've been warned**.

### Support
None. This tool is provided free-of-charge.
To be clear here: I spent time creating it, you are using it - I don't *owe* you anything. I may change it however and whenever I want to. However, if you find a bug or miss a feature, you're very welcome to [fork this repository](https://help.github.com/articles/fork-a-repo/), fix it and send over a [pull-request](https://help.github.com/articles/using-pull-requests/).


### Missing features & custom development
[You are very welcome to hire me](https://joergbattermann.com/).

## How-To

### Download

Head over to [Releases](https://github.com/jbattermann/JB.Id3Editor/releases), download the latest version and unzip the .zip file.
It'll contain, among other file(s) and folder(s) to operate, a '*JB.Id3Editor.exe*' file - that's the .exe you'll be using.

### System requirements

This tool is written in .Net 4.5 and is full [Mono](http://www.mono-project.com/download/) compatible (tried it with 4.2.x) and therefore runs (*tested*) on **Windows**, **Mac OS X** as well as (*probably, untested*) on **Linux** as long as you have a .Net 4.5 runtime on your system.

### Known Limitations and Issues

* the ```--targetpath``` parameter does not work properly if the specified path has a trailing ```\```. This is stupid, but [a known bug](https://github.com/gsscoder/commandline/issues/240) of the [Command Line Parser Library](https://github.com/gsscoder/commandline).
    * Known Workaround a): specify the ```--targetpath``` parameter without a trailing ```\``` or
    * Known Workaround b): specify the ```--targetpath``` parameter as the very last parameter

### Functionalities

This tool currently provides the following commands and options which may or may not grow in the future:

```
  clearcovers    Clears all existing album cover(s) from target file(s).

  writecovers    Writes album cover(s) to target file(s) as specified in
                 .ini(s).

  help           Display more information on a specific command.

  version        Display version information.
```

**Please note**: all commands and options are *case-sensitive*!


#### Clear existing Cover art (a.k.a 'clearcovers')

The ```clearcovers``` command allows the tool to clear all existing cover art (front and back) in the target file(s). This scans all specified file(s) for existing cover art and if one or more were found, removes them. Simple as that. The command's options look like this:

```
  --targetpath                Required. Path to target file or directory.

  --recursive                 (Default: false) If --targetpath is a Directory,
                              enabling --recursive will traverse into
                              sub-directories, too.

  --searchfilter              (Default: *.mp3) If --targetpath is a Directory,
                              this will be used to find the corresponding
                              file(s) to process.

  --maxdegreeofparallelism    (Default: 1) Specifies the maximum level of
                              concurrency. If you have fast I/O multiple CPU
                              cores, increasing this value will improve runtime.
```

##### Sample Usage

###### Clear cover from a single file:
```
d:\JB.Id3Editor.exe clearcovers --targetpath="D:\My_Awesome_Song.mp3"
```

###### Clear cover from all files directly in a folder (non-recursive):
```
d:\JB.Id3Editor.exe clearcovers --targetpath="D:\My_Awesome_Album_Folder\"
```

###### Clear cover from all files in a folder and its sub-folders (a.k.a recursively):
```
d:\JB.Id3Editor.exe clearcovers --recursive --targetpath="D:\My_Awesome_Music_Collection\"
```

or, if your storage device and amount of cpu cores permits it, run the task(s) in parallel via the ```--maxdegreeofparallelism``` option, i.e.:

```
d:\JB.Id3Editor.exe clearcovers --maxdegreeofparallelism=4 --recursive --targetpath="D:\My_Awesome_Music_Collection\"
```
... this will run up to 4 parallel tasks & therefore clearing 4 files in parallel.


If you want to run the tool against file(s) other than .mp3 ones, you can use the ```--searchfilter``` option, i.e.:
```
d:\JB.Id3Editor.exe clearcovers --searchfilter="*.empeethree" --targetpath="D:\My_Awesome_Album_Folder\"
```


#### Write (front) cover art to files (a.k.a 'writecovers')

The ```writecovers``` command allows to write (front) cover art to either a single file or in bulk to files in a directory. Besides a [Default Cover](https://github.com/jbattermann/JB.Id3Editor/blob/master/JB.Id3Editor/Covers/DefaultCover.png),
a [mapping .ini file](https://github.com/jbattermann/JB.Id3Editor/blob/master/JB.Id3Editor/UserMappings.ini) for [ID3 genres](http://id3.org/id3v2.3.0#Appendix_A_-_Genre_List_from_ID3v1)
to cover files allows custom cover arts to be written based on the [genre Id3 tag](http://id3.org/id3v2.3.0#Appendix_A_-_Genre_List_from_ID3v1) in the corresponding file.

The available options are:

```
  --targetpath                Required. Path to target file or directory.

  --custommappingsfile        (Default: <empty>) Full path to custom Mappings.ini
                              file.

  --force                     (Default: false) If enabled, existing cover art
                              in .mp3(s) will be overwritten instead of
                              skipping these file(s).

  --recursive                 (Default: false) If --targetpath is a Directory,
                              enabling --recursive will traverse into
                              sub-directories, too.

  --searchfilter              (Default: *.mp3) If --targetpath is a Directory,
                              this will be used to find the corresponding
                              file(s) to process.

  --maxdegreeofparallelism    (Default: 1) Specifies the maximum level of
                              concurrency. If you have fast I/O multiple CPU
                              cores, increasing this value will improve runtime.
```


##### Sample Usage

###### Write default front cover to a single file that has no pre-existing cover art:
```
d:\JB.Id3Editor.exe writecovers --targetpath="D:\My_Awesome_Song_Without_CoverArt.mp3"
```
By default the tool skips over file(s) that have pre-existing cover art but you can...

###### Write default cover to a single file that has a pre-existing cover art:
```
d:\JB.Id3Editor.exe writecovers --force --targetpath="D:\My_Awesome_Song_Without_CoverArt.mp3"
```

The ```--force``` option forces the tool to ignore the check for pre-existing cover art and write the (default) cover art to the file(s) in any way.

###### Write default cover to all files in a target directory and its sub-directories, overwriting existing cover art and doing so with 4 parallel operations:
```
d:\JB.Id3Editor.exe writecovers --force --recursive --maxdegreeofparallelism=4 --targetpath="D:\My_Awesome_Music_Collection\"
```

###### Write cover art per genre to all files in a target directory and its sub-directories, overwriting existing cover art:
```
d:\JB.Id3Editor.exe writecovers --force --recursive --custommappingsfile="./My_Custom_Mappings.ini" --targetpath="D:\My_Awesome_Music_Collection\"
```

The ```--custommappingsfile``` option and its specified file can contain custom mappings of the genre ID3 tag to different cover art.

This .ini file must adhere to the .ini format given in the provided '[UserMappings.ini](https://github.com/jbattermann/JB.Id3Editor/blob/master/JB.Id3Editor/UserMappings.ini)' file:

* (Lines with) Comments start with the '#' character
* Mappings must be inside the '```[Genres]```' section, one mapping at a line and look like this:
  * '```<Genre Name> = <Full or relative path to cover art file>```', i.e.:
    * ```Alternative = ./Covers/Alternative.png```
    * ```Classical = ./Covers/Classical.png```
    * ```Pop = ./Covers/Pop.png```
    * ...

The tool comes with a [sample .ini file](https://github.com/jbattermann/JB.Id3Editor/blob/master/JB.Id3Editor/UserMappings.ini) you can use as a base to create your own.
If the genre tag inside a file does not match any of the specified ones in the .ini file, it'll fallback to the default one. That's why the default one is mandatory and included and specified in the [DefaultMappings.ini](https://github.com/jbattermann/JB.Id3Editor/blob/master/JB.Id3Editor/DefaultMappings.ini).
You can certainly create your own default cover file and specify it there, but a default one must exist or the tool won't start processing the file(s).

## Acknowledgements

This tool uses the following libraries and I'd like think their authors and contributors for providing them - good stuff!

* [TagLib#](https://github.com/mono/taglib-sharp) a.k.a. [taglib](https://www.nuget.org/packages/taglib/)
* [INI Parser](https://github.com/rickyah/ini-parser) a.k.a. [ini-parser](https://www.nuget.org/packages/ini-parser/)
* [Command Line Parser Library](https://github.com/gsscoder/commandline) a.k.a. [CommandLineParser](https://www.nuget.org/packages/CommandLineParser)

## Licence
See the [LICENSE file](https://github.com/jbattermann/JB.Id3Editor/blob/master/LICENSE)