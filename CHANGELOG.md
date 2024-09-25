# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.6.1-beta.4] - 2024-09-25

### Added

- Toggle for persistent poses

### Changed

- "Tracking Override" toggle now named "Settings", and includes "Persistent Poses" toggle

## [0.6.1-beta.3] - 2024-09-21

### Fixed

- Fixed persistent pose not being correctly restored

## [0.6.1-beta.1] - 2024-09-21

### Added

- Optional persistent pose support (toggle in Factory)

### Fixed

- "spam" the pose locking behaviour to mitigate broken animators unlocking poses randomly (temporarily applied forcefully. will become optional in the future)

## [0.5.0] - 2024-05-18

### Changed

- Moved pose engine to Base animator layer

### Fixed

- Fixed mirrored poses not being correctly toggled