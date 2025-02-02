# Test Plan and Results

## Overall Test Plan

The testing strategy for this project focuses on ensuring the integrity and functionality of core game mechanics, including the turn-based system, action constraints, character abilities, environmental interactions, and overall game flow. The strategy utilizes both unit and integration testing, with black-box testing primarily used to validate outputs against expected behavior, and white-box testing applied for procedural systems like dungeon generation. Functional testing is prioritized to verify mechanics such as movement, combat, and event triggering, while performance and edge case testing are conducted for scenarios like game-over conditions and multiplayer synchronization. This strategy ensures thorough coverage of all critical features and edge cases to deliver a robust and engaging gameplay experience.

## Test Case Descriptions

**Player Turn Highlight**
- Purpose: Validate turn-based movement highlighting
- Description: Test that the movement range is correctly highlighted when it’s the player’s turn
- Inputs: Player turn begins with a character at a specific position
- Expected Outputs: A circle appears around the player, highlighting tiles they can move to within their movement range
- Case Type: Normal
- Test Type: Blackbox
- Category: Functional
- Level: Unit

**Player Movement Validation**
- Purpose: Validate player movement within allowed range
- Description: Test that the player can only move to tiles within their highlighted range
- Inputs: Player clicks on a tile within and outside their movement range
- Expected Outputs: Movement is allowed only to tiles within range. Invalid movement attempts are ignored
- Case Type: Boundary
- Test Type: Blackbox
- Category: Functional
- Level: Unit

**Two Action Limit**
- Purpose: Ensure players can only take two actions per turn
- Description: Validate that the player can perform one combat and one movement action, or two movement actions, but no more
- Inputs: Player performs various action combinations during their turn
- Expected Outputs: After two actions, the turn ends automatically
- Case Type: Normal
- Test Type: Blackbox
- Category: Functional
- Level: Unit

**Inventory Usage**
- Purpose: Validate functiuonality of using items during a turn
- Description: Ensure players can correctly use consumable inventory items (e.g., healing potions) during their turn
- Inputs: Player selects a consumable item from the inventory
- Expected Outputs: Item effect is applied (e.g., health is restored), and the item is removed from the inventory
- Case Type: Normal
- Test Type: Blackbox
- Category: Functional
- Level: Unit

**Inventory Modifiers**
- Purpose: Test permanent and temporary modifiers from inventory items
- Description: Validate that items granting modifiers (e.g., +3 attack damage for 5 turns) apply their effects correctly
- Inputs: Player acquires a modifier item
- Expected Outputs: Modifier is reflected in the player’s stats and persists throughout the session or drops in usage
- Case Type: Normal
- Test Type: Blackbox
- Category: Functional
- Level: Integration

**Ability Cooldowns**
- Purpose: Validate cooldown mechanics for player abilities
- Description: Ensure abilities with cooldowns cannot be used until the cooldown expires
- Inputs: Player uses an ability, then attempts to use it again before and after cooldown time
- Expected Outputs: Ability is unavailable during cooldown, including visual indicator
- Case Type: Normal
- Test Type: Blackbox
- Category: Functional
- Level: Unit

**One-Time Use Ability**
- Purpose: Test one-time use special abilities
- Description: Ensure that one-time use abilities are consumed after activation
- Inputs: Player activates a one-time use ability during their turn
- Expected Outputs: Ability is removed from the available actions list for the rest of the game
- Case Type: Normal
- Test Type: Blackbox
- Category: Functional
- Level: Unit

**Environmental Interactions**
- Purpose: Test enviornmental interaction functionality
- Description: Ensure players can interact with environmental itme during their turn which perform certain actions
- Inputs: Player clicks on or moves into interaction area
- Expected Outputs: Environment is changed (e.g., door opens, trap deals damage) and potential effects are applied
- Case Type: Normal
- Test Type: Blackbox
- Category: Functional
- Level: Integration

**Adventure Goal Validation**
- Purpose: Validate selected adventure goals
- Description: Ensure that the selected goal (e.g., kill 12 monsters) is correctly tracked and enforces session completion
- Inputs: Players progress through the level and complete the objective
- Expected Outputs: Game ends with success or failure depending on goal achievement
- Case Type: Normal
- Test Type: Blackbox
- Category: Functional
- Level: Integration

**Audio-Visual Feedback**
- Purpose: Validate animations and sound effects play at appropriate times during gameplay
- Description: Ensure that sound effects and animations play correctly when triggered for their specific actions
- Inputs: User interacts with game (e.g., moving, dealing damage, interacting with UI)
- Expected Outputs: Corresponding audio and visual cues are played
- Case Type: Normal
- Test Type: Blackbox
- Category: Functional
- Level: Unit

**XP and Level Up**
- Purpose: Validate XP gain and level up mechanics
- Description: Ensure players gain respective XP for defeating monsters and can only level up when conditions are met
- Inputs: User defeats enemy with XP rewards
- Expected Outputs: XP is tracked and players level up at appropriate times
- Case Type: Normal
- Test Type: Blackbox
- Category: Functional
- Level: Integration

**Turn and Action Enforcement**
- Purpose: Enforce turn-based restrictions
- Description: Ensure players can only perform actions on their turn
- Inputs: Player attempts to perform an action during another player's turn
- Expected Outputs: Actions are not performed and proper message is displayed
- Case Type: Normal
- Test Type: Blackbox
- Category: Functional
- Level: Unit

**Save and Load**
- Purpose: Ensure accurate saving and loading of game progress
- Description: Validate game state (turn order, HP, layout, progress, etc.) are saved and loaded properly
- Inputs: Mid-level game state
- Expected Outputs: Reloaded game matches saved state
- Case Type: Boundary
- Test Type: Blackbox
- Category: Functional
- Level: Integration

**Healing Surge Activation**
- Purpose: Validate healing surge activates on dead player's turn
- Description: Ensure that a healing surge is used the following turn after a player's HP reaches 0
- Inputs: Player HP reduced to 0
- Expected Outputs: Healing surge revives player to predefined HP value or ends game if none remain
- Case Type: Boundary
- Test Type: Blackbox
- Category: Functional
- Level: Integration

**Combat Validation**
- Purpose: Validate combat outcomes based on distance, rolls, and status
- Description: Ensure combat results are calculated and reported correctly
- Inputs: Player attacks enemy with specific stats
- Expected Outputs: Accurate outcomes and damage application
- Case Type: Normal
- Test Type: Whitebox
- Category: Functional
- Level: Unit

**Stress Test Performance**
- Purpose: Validate game performance under high stress
- Description: Test game stability with many AI entities and on-screen particles
- Inputs: Dungeon populated with numerous entities and an effect with particles applied to all entities
- Expected Outputs: Smooth performance and no excessive frame drops or crashes
- Case Type: Abnormal
- Test Type: Blackbox
- Category: Performance
- Level: Integration

**UI Updates**
- Purpose: Test user interface functionality
- Description: Verify UI elements such as health bars and turn icons are updated during gameplay
- Inputs: Actions affecting player stats or game state
- Expected Outputs: UI elements reflecting game state reflect the correct game state
- Case Type: Normal
- Test Type: Blackbox
- Category: Functional
- Level: Unit

**Multiplayer Sync**
- Purpose: Validate multiplayer synchronization
- Description: Ensure consistent game states accross players in the same session
- Inputs: Player performs action
- Expected Outputs: All players see synchronized game state
- Case Type: Boundary
- Test Type: Blackbox
- Category: Functional
- Level: Integration

**AI Behavior**
- Purpose: Ensure AI enemy behaviors function properly
- Description: Validate that enemies move, attack, and react properly to the game state
- Inputs: It is an enemy turn
- Expected Outputs: Enemy displays correct behavior
- Case Type: Normal
- Test Type: Blackbox
- Category: Functional
- Level: Unit

**Dungeon Generation**
- Purpose: Test dungeon generation logic
- Description: Generate dungeon and validate room placement, connectivity, enemy spawns, and loot distribution
- Inputs: Procedural generation seed
- Expected Outputs: Complete, connected, and playuable dungeon layout with properly placed enemies and loot
- Case Type: Normal
- Test Type: Whitebox
- Category: Functional
- Level: Integration

### Test Case Matrix

| Test Name                    | Case Type     | Test Type     | Category       | Level            |
|------------------------------|---------------|---------------|----------------|------------------|
| Player Turn Highlight        | Normal        | Blackbox      | Functional     | Unit             |
| Player Movement Validation   | Boundary      | Blackbox      | Functional     | Unit             |
| Two Action Limit             | Normal        | Blackbox      | Functional     | Unit             |
| Inventory Usage              | Normal        | Blackbox      | Functional     | Unit             |
| Inventory Modifiers          | Normal        | Blackbox      | Functional     | Integration      |
| Ability Cooldowns            | Normal        | Blackbox      | Functional     | Unit             |
| One-Time Use Ability         | Normal        | Blackbox      | Functional     | Unit             |
| Environmental Interactions   | Normal        | Blackbox      | Functional     | Integration      |
| Adventure Goal Validation    | Normal        | Blackbox      | Functional     | Integration      |
| Audio-Visual Feedback        | Normal        | Blackbox      | Functional     | Unit             |
| XP and Level Up              | Normal        | Blackbox      | Functional     | Integration      |
| Turn and Action Enforcement  | Normal        | Blackbox      | Functional     | Unit             |
| Save and Load                | Boundary      | Blackbox      | Functional     | Integration      |
| Healing Surge Activation     | Boundary      | Blackbox      | Functional     | Integration      |
| Combat Validation            | Normal        | Whitebox      | Functional     | Unit             |
| Stress Test Performance      | Abnormal      | Blackbox      | Performance    | Integration      |
| UI Updates                   | Normal        | Blackbox      | Functional     | Unit             |
| Multiplayer Sync             | Boundary      | Blackbox      | Functional     | Integration      |
| AI Behavior                  | Normal        | Blackbox      | Functional     | Unit             |
| Dungeon Generation           | Normal        | Whitebox      | Functional     | Integration      |
