# Pose Engine
Pose engine is a tool that makes it easy to add custom poses, and complex locomotion systems to your vrchat avatar. It aims to be easy to use, while remaining as flexible as possible, and is **entirely non-destructive.**

## Features
- **Easy to use**: Pose engine is designed to be easy to use, and requires **no coding or animator skills** to get started with.
- **Non-destructive**: Pose engine does not modify your avatar in any way, and can be removed at any time.
- **Accessible**: All components are simple to get started with.
- **Customizeable/Flexible**: While every components are easy to use, they are also very flexible, and can be used to create a wide variety of locomotion setups.
- **Extendable**: Pose engine builds off NDMF, and can be extended with custom components. Effort is also made to make the generated assets human-friendly for debugging and tinkering purposes.

## Installation
### VRC Creator Companion (Recommended)
Pose engine is available from [mitsukaki's VPM repo](https://vpm.mitsukaki.com/), and can be installed by adding the VPM repository to VRC Creator Companion, and installing the `pose-engine` package.

### Releases
Pose engine is also available as a unity package or as a `.zip` from the [releases page](https://github.com/mitsukaki/pose-engine/releases).

To use the unity package, simply download and import it into your project. For the `.zip`, unpack the contents to a folder and move it to your project's `Assets` folder.

## Getting Started
Pose engine requires 2 steps to get started:
1. **Add the `PoseEngine` component to your scene**: This component is the main entry point for the pose engine, letting you alter settings for pose-engine, add custom components, force builds (to speed up build times), and more.

2. **Add 1 or more PoseEngine components (prefixed with `PE`) to your avatar**: These components are the building blocks you can use to apply poses to your avatar.

The simplest pose component is the `PE Simple Pose` component, on which you specify as many poses as you want in a list, and it generates a simple menu to switch between them.