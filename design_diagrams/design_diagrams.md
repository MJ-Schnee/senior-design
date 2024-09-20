# Design Diagrams

Project Snow aims to translate mechanics from board games such as the D&D Adventure System Board Games into a video game format, enabling players to explore dungeons, battle enemies, and experience quests and campaigns. The following diagrams provide a progressive breakdown of the game system from a high-level overview to the detailed internal workings, with a focus on game input processing and interactive outputs to players.

## Diagram 0: High-Level System Overview

This diagram provides a top-level view of the game system, identifying the primary inputs, processing logic, and outputs involved in gameplay.

- **Description**:
  - Shows the overall flow of information, starting from player inputs (such as hero movements and actions), game setup, and environmental data, moving through the core game processing, and finally resulting in visual and audio feedback.
  - **Input Section**: Captures player commands, game setup information, and multiplayer communication data.
  - **Process Section**: Details how the game processes these inputs to control gameplay elements like character actions, AI behaviors, and event handling.
  - **Output Section**: Highlights the game’s response in the form of visual and audio feedback, including updated game states, combat outcomes, and player notifications.
- **Purpose**: Provides a high-level understanding of how the game translates player actions and game events into interactive outputs.

## Diagram 1: Subsystem Interaction

This diagram focuses on the interaction between various subsystems within the game, emphasizing their communication with the Game Logic System.

- **Description**:
  - Depicts how different subsystems, such as AI, Event System, Multiplayer, and Rendering, interact with the central Game Logic System.
  - **Subsystems Overview**:
    - **Input Handling Subsystem**: Processes player commands before they are passed to the Game Logic System.
    - **Game Logic System**: The core hub that manages game state changes and coordinates between different subsystems.
    - **Subsystems Interaction**: Illustrates the flow of information between the Game Logic System and other subsystems (AI, Event System, Multiplayer, Visual Rendering, and Audio).
  - **Purpose**: Demonstrates how player inputs are processed and managed within the game system, and how the various subsystems contribute to creating a cohesive gameplay experience.

## Diagram 2: Detailed System Architecture

This diagram provides a more in-depth view of the system’s architecture, showing specific modules and their interactions within the game.

- **Description**:
  - Details the flow of commands and data between individual modules, including user input handling, game logic processing, AI behaviors, and multiplayer synchronization.
  - **Components Overview**:
    - **UI Manager**: Manages user interactions and camera control, updating the interface based on game state changes.
    - **Command Processor**: Interprets player commands and coordinates with the Game Logic Engine.
    - **Game Logic Engine**: Coordinates gameplay mechanics and manages interactions between AI behaviors, visual rendering, and multiplayer synchronization.
    - **Multiplayer Synchronization Module**: Ensures consistent game state updates across connected players.
    - **AI Behavior Module**: Controls enemy and non-player character actions based on predefined rules and real-time game data.
  - **Purpose**: Provides a comprehensive view of the internal workings of the game system, detailing how user inputs are processed and translated into game actions and outputs through the various interconnected modules.
