CARD KINGDOM SOLITAIRE

A polished single-player 2D card game built in Unity where players strategically play cards by matching suit or higher rank.


Candidate Information

- Name: Vibhav Singh
- Unity Version: Unity 6
- Time Taken: ~15 hours
- Submission Date: 24/06/2026]


How to Play

1. Click any card from your hand to play it
2. Valid play = Same Suit OR Higher Rank than center card
3. Each valid play awards +10 points
4. Win: Clear all cards from deck and hand
5. Lose: No valid moves available


📁 Project Structure
Assets/
├── _Game/  Game-specific assets
│ ├── Sprites/  UI sprites, backgrounds
│ ├── Fonts/  Custom fonts
│ └── Animations/  
├── Audio/
│ ├── Music/  Background music
│ └── SFX/  Sound effects
├── Resources/
│ └── Cards/  52 card sprites (runtime loaded)
├── Scripts/
│ ├── Core/
│ │ ├── Card.cs  Card data class (Suit, Rank, validation)
│ │ ├── DeckManager.cs  Deck creation, shuffling, drawing
│ │ └── GameManager.cs  Game logic, state, score management
│ ├── UI/
│ │ ├── UIManager.cs  All UI updates and management
│ │ ├── CardUI.cs  Card visuals and interaction
│ │ ├── MainMenuUI.cs  Main menu functionality
│ │ ├── ButtonAnimator.cs  Smooth button animations
│ │ └── ButtonClickSound.cs  Button click sound handler
│ └── Utils/
│ ├── HighScoreManager.cs  PlayerPrefs high score system
│ └── AudioManager.cs  Music and SFX management
├── Scenes/
│ ├── MainMenu.unity
│ └── Gameplay.unity
└── Prefabs/
└── CardPrefab.prefab




FEATURES IMPLEMENTED

Core Features (As per requirements)
- Standard 52-card deck (4 suits × 13 ranks)
- Fisher-Yates shuffle algorithm
- Initial distribution (1 center + 5 hand cards)
- Card play validation (Same Suit OR Higher Rank)
- Score system (+10 per valid move)
- Auto-draw mechanic after valid play
- Invalid move feedback message
- Win condition detection
- Lose condition detection
- All 4 required screens (Main Menu, Gameplay, Victory, Game Over)

BONUS FEATURES IMPLEMENTED
- High Score System using PlayerPrefs (persistent across sessions)
- Smooth card hover animations
- Button hover and press animations
- NO VALID MOVES! transition message before Game Over screen
- Background music in both scenes
- Card click sound effects
- Button click sound effects
- Premium UI design with custom themed assets
- Visual feedback for all interactions
- Rule reminder display during gameplay
- Color-coded UI theming (gold accents, themed panels)


DESIGN DECISIONS & ENHANCEMENTS

Enhanced Lose Condition
The original assignment specifies the lose condition as: *"Deck is empty AND no 
valid moves in hand."* However, this created a problematic scenario where players 
could get stuck mid-game (with cards still in deck but no playable cards in hand), 
unable to progress.

Implementation Decision: I enhanced the lose condition to trigger whenever 
the player has no valid moves available, regardless of deck status. This provides 
a smoother and more intuitive player experience.

Smooth Transition: Instead of abruptly showing the Game Over screen, I added 
a 1.8-second "NO VALID MOVES!" message animation before transitioning, giving 
the player time to understand why the game ended.

Architecture Choices
- Singleton Pattern for managers ensures single source of truth
- Event-driven communication decouples systems for maintainability
- DontDestroyOnLoad AudioManager ensures music continuity between scenes
- Resources folder for dynamic sprite loading at runtime


TECHNICAL HIGHLIGHTS

Code Quality
- Comprehensive XML documentation comments
- Consistent naming conventions (PascalCase for classes/methods, camelCase for fields)
- Encapsulation with proper access modifiers
- Defensive coding with null-safety checks
- SerializeField for clean Inspector workflow

Unity Best Practices
- Script Execution Order configured for proper initialization
- Resource loading optimization with Resources.Load
- Canvas Scaler for responsive UI across resolutions
- Prefab-based card instantiation
- Coroutines for time-based animations

Performance Considerations
- Object pooling not needed due to small card count (5 active)
- Efficient hand updates using Destroy with safety checks
- Audio streaming for music, decompress on load for SFX


ASSETS USED

- Card Sprites: [Kenney Boardgame Pack](https://kenney.nl/assets/boardgame-pack) (CC0 - Free)
- UI Icons: [Flaticon](https://www.flaticon.com) (Free with attribution)
- Background Music: [Pixabay Music](https://pixabay.com/music/) (Royalty-free)
- Sound Effects: [Pixabay Sound Effects](https://pixabay.com/sound-effects/) (Royalty-free)
- Fonts: Unity Default (LiberationSans) & few imported from GOOGLE FONTS


HOW TO RUN

1. Open the project in Unity 6 
2. Open the scene: `Assets/Scenes/MainMenu.unity`
3. Press the Play button in Unity Editor
4. Click "PLAY" button to start the game
5. Click any card from your hand to play it!


EVALUATION CRITERIA COVERAGE

| Criteria | Weight | Implementation |
|----------|--------|----------------|
| Game Logic | 35% | Complete game rules, validation, win/lose conditions, scoring system |
| Code Quality & Architecture | 25% | OOP principles, Singleton pattern, event-driven, documentation |
| Unity Fundamentals | 15% | Prefabs, scenes, UI system, components, lifecycle methods |
| UI Implementation | 15% | All 4 screens polished with animations and feedback |
| Project Organization | 10% | Clean folder structure, proper naming, modular scripts |


Thank You!

Thank you for the opportunity to work on this assignment. 
Looking forward to your feedback!

Best regards,
Vibhav Singh




