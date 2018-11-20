### This project is discontinued and the code is pretty messy as I never took the time to clean it up. Feel free to use this project for your needs under the granted license.

# BetterDeck

BetterDeck is supposed to be an alternative program for the Elgato Stream Deck. It was created because the original software from Elgato doesn't function on operating systems other than Windows 10.

# Features

- Auto (re-)connect to Stream Deck
- Profile management (save, load, delete)
- Multi-action support for buttons
- Visual button customization: Add an image, a background and a custom text (including font size, color and offset alignment)
- Customizable brightness
- Press keys or send texts (to Windows)
- Support for teamspeak (un/mute input and output)
- Minimizes to tray instead of the Windows taskbar

# To be implemented

- Open Broadcaster support (change scenes, un/mute input and output, ...)
- Twitch API support (send chat messages, use commands, ...)
- A lot of UI for stuff that currently has to be changed in the code

# Bugs

- Application crash when triggering an empty action
- Probably some other bugs and flaws

## Important libraries

- [StreamDeckSharp](https://github.com/OpenMacroBoard/StreamDeckSharp)
- [InputSimulator](https://archive.codeplex.com/?p=inputsimulator)
- [TS3QueryLib.Net](https://github.com/Scordo/TS3QueryLib.Net)
