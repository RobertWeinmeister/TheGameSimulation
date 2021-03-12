# The Game Simulation

This is a simulation of the card game "The Game" ([Publisher](https://pandasaurusgames.com/products/the-game-kwanchai-moriya-edition) and [BoardGameGeek](https://www.boardgamegeek.com/boardgame/173090/game)). It simulates games using player implementations with different rulesets to allow the analysis of different player rules.

## 1 Motivation

Whilst playing the physical version of this game, a discussion arose whether a very simple set of rules to follow would suffice and how much allowed communication is necessary to succeed. This simple implementation was written to see the impact of different employed rules.

## 2 Implementation

The implementation of the game follows the [standard rules](https://nsv.de/wp-content/uploads/2018/05/the-game-english.pdf). Five different player rulesets are implemented. Each ruleset is simulated for one to five players and the results of all runs are aggregated.

### 2.1 Simplest ruleset with no communication

The simplest ruleset with three rules:
1. Play every possible backwards trick.
2. Play every card with a distance of up to 3.
3. If more cards need to be played, play them in descending order of distance until the minimum number of cards to play is reached.
 
### 2.2 Simplest rules with added minimal communication

Basic communication is added. At the start, a rough agreement is reached about who should start. The most important addition is if one could play a backwards trick on a row in one’s turn, this is communicated to every other player.

The slightly modified three rules are then:
1. Play every possible backwards trick.
2. Play every card with a distance up to 5 unless blocked by another player's possible backwards trick.
3. If more cards need to be played, play them in descending order of distance until the minimum number of cards to play is reached.
	
### 2.3 Best plays with no communication

Instead of just going for the immediate "best" card, the whole range of available plays is analyzed to go for the one that delivers the least numerical increase of the top cards of the rows (the mentioned score change).

This ruleset has two rules:
1. Play every play with a score change up to 4. (This includes all backwards tricks.)
2. If more cards need to be played, play them by the lowest score until the minimum number of cards to play is reached.

### 2.4 Best plays with added minimal communication

Again, basic communication is added (see 2.2). A communicated backwards trick now does not completely block a row but adds a score penalty of 21.

The slightly modified rules are then:
1. Play every play with a score change up to 5 if two cards have to be played, otherwise up to 8. A row with DoNotPlayHere has a score penalty of 21.
2. If more cards need to be played, play them by the lowest score until the minimum number of cards to play is reached. A row with DoNotPlayHere has a score penalty of 21.

### 2.5 Extended ruleset
 
For comparison, the communication of ruleset 2.4 was extended to include:
 * Communicate good moves (moves with distance up to 4 possible)
 * Communicate bad moves (only moves with a distance of 14 or greater are possible)
 * Communicate, if one could not play with the current rows

This was incorporated into the ruleset.
 
## 3 Results

For the five implementations and each number of players, the game was run 10,000 times. excellent% is the percentage of games with less than 10 cards left, perfect% the percentage of games with no cards left.

| Ruleset | # of players | avg. # of cards left | excellent% | perfect% | min # of cards left | max # of cards left |
| - | :-: | :-: | :-: | :-: | :-: | :-: |
| 2.1 (simplest, no communication) | 1 | 22.0 | 12 | 1 | 0 | 54 |
|  | 2 | 13.8 | 44 | 3 | 0 | 56 |
|  | 3 | 17.9 | 32 | 1 | 0 | 63 |
|  | 4 | 13.2 | 47 | 1 | 0 | 70 |
|  | 5 | 11.2 | 53 | 0 | 0 | 61 |
| 2.2 (simplest, simple communication) | 1 | 23.4 | 10 | 1 | 0 | 55 |
|  | 2 | 11.7 | 53 | 4 | 0 | 54 |
|  | 3 | 14.5 | 44 | 1 | 0 | 65 |
|  | 4 | 10.0 | 62 | 1 | 0 | 61 |
|  | 5 | 8.7 | 68 | 1 | 0 | 58 |
| 2.3 (best plays, no communication) | 1 | 16.4 | 31 | 5 | 0 | 51 |
|  | 2 | 9.0 | 65 | 6 | 0 | 50 |
|  | 3 | 13.2 | 49 | 1 | 0 | 58 |
|  | 4 | 9.9 | 63 | 1 | 0 | 56 |
|  | 5 | 9.2 | 63 | 0 | 0 | 55 |
| (best plays, simple communication) | 1 | 17.1 | 28 | 5 | 0 | 50 |
|  | 2 | 5.5 | 82 | 14 | 0 | 53 |
|  | 3 | 7.0 | 79 | 6 | 0 | 57 |
|  | 4 | 4.9 | 91 | 5 | 0 | 49 |
|  | 5 | 5.2 | 90 | 3 | 0 | 46 |
| (best plays, full communication) | 1 | 17.1 | 28 | 4 | 0 | 54 |
|  | 2 | 3.8 | 88 | 24 | 0 | 42 |
|  | 3 | 5.6 | 83 | 10 | 0 | 49 |
|  | 4 | 4.1 | 94 | 8 | 0 | 57 |
|  | 5 | 4.6 | 92 | 5 | 0 | 67 |

Leaving aside the results from a single player:

The simplest ruleset with no communication leads already to an excellent result in 45% of the games. By mainly communicating possible backwards tricks to prevent other players from playing on this row, an excellent result can be achieved in 57% of the games.

By using one's cards to play the best set of moves instead of just going for the immediate "best" card, an excellent result is reached in 60% of the games. With the added simple communication of possible backwards tricks, this goes up to 86%. Using a full set of communication increases this to 90%.

## 4 Conclusions

Generally, the more cards in play, the better the results. Thus more players get better results, except from two to three players, as the number of hand cards for each player gets reduced.

The simplest ruleset is already enough to get an excellent result in 45% of the games and finish games with no cards left. By requiring the players to analyze their hand cards to get the best possible play, this can be increased to 60%. With the simplest communication added, this is boosted to 86%. For two players, 14% of the games are even perfect.

Any further communication increases the results, but marginally. Interestingly, even then some games end with more than 50 cards left.

Concluding, simply playing a certain quality of plays with communicating backwards tricks is enough to reliably excel in this game.

## 5 Usage

Simply run the application. The settings can be configured using the application parameters which are explained at the start of each run of the application.

For each specified player ruleset, the game is run with one to five players. For the result of each of these, the average number of cards left, the percentage of excellent results (less than ten cards left), the percentage of perfect results (no cards left), and the minimum and the maximum number of cards left are shown.

### 5.1 Implement your own player

First, add the player name to the list of available players in the class **AvailablePlayer**. Second, add the player construction in the class **PlayerFactory** by adding and setting the individual rules and also add the player in the method **GetPlayers** there.

### 5.2 Implement your own rules

The three types of player rules are set in the three corresponding classes **PlayerStartDecisionRules**, **PlayerMoveRules** and **PlayerCommunicationRules**.

#### 5.2.1 PlayerStartDecisionRules

The available information is the cards in hand and the current round of start communication. The result has to be a **PlayerStartDecision**.

#### 5.2.2 PlayerMoveRules

The available information is the cards in hand and the **PlayerInformation** (Number of cards played this turn, cards still to play this turn, the rows of cards and the communication). The result has to be a **PlayerMove**.

#### 5.2.3 PlayerCommunicationRules

The available information is the cards in hand and the **PlayerInformation** (Number of cards played this turn, cards still to play this turn, the rows of cards and the communication). The result has to be a list of **PlayerCommunication**.
