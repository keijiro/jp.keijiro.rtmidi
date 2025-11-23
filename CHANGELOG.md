# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [2.2.0] - 2025-11-23

### Added

- Added error callback support (with explicit cancellation).

### Changed

- Synced the RtMidi C wrapper with upstream.
- Added MidiBase abstract base class to share the base functionality between
  MidiIn and MidiOut.
- Refactored the managed callback bridge for IL2CPP.
- Refreshed native plugin binaries/import settings for Unity 2022.3 across all
  supported platforms; improved build scripts including Emscripten detection
  for Web.
- Migrated the package release workflow for the signed package process.

### Fixed

- Avoided recursion in `CompiledApiByName` P/Invoke and tightened error handling
  paths when releasing callbacks.
- Fixed Linux test startup issues caused by virtual MIDI ports being enumerated
  as usable devices.

## [2.1.3] - 2025-05-28

### Changed

- Regenerated the Windows RtMidi.dll import settings with Unity 2022.3 to match
  the current platform defaults.

## [2.1.2] - 2025-05-14

### Fixed

- Rewrote Android MIDI polling so concatenated packets with multiple messages
  are handled without dropping events.
- Adjusted MidiInTest to skip unsupported MIDI message types and avoid false
  positives.

### Added

- Added Android Logcat as a dependency to help with Android runtime debugging.
- Added pitch bend and aftertouch checks to the MidiInTest sample scene.

### Changed

- Updated mobile build settings and refreshed the README to reflect the current
  test setup.
