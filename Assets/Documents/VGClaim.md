# VG Claim - Final Project

  

**Name:**Astrid Skillingstad

**Game title:** Patient Unknown

**GitHub repo:** https://github.com/teukstrid/PatientUnknown

**Final build link:**

  

---

  

## The Rules

  

- At least **6** Toolbox items in total

- At least **4** of the 6 from the Code list (items 1 to 14)

- The **3 required items** are already in the table: 3, 10 and 18

- An item counts when the player experiences it in your game

- Your code is commented in your own words

  

## Where to Find It

  

- **Code items:** the script, the function, and the line number. Example: `PlayerHealth.cs`, `TakeDamage()`, line 24

- **Design items:** the scene and the object. Add a screenshot in `Assets/Documentation/` and write its file name

- **Item 18 (difficulty):** the date of your DesignLog.md entry with the before and after numbers

  

## My Items

  

| # | Item | Where to find it | Because I built ___, the player ___. |

|---|---|---|---|

| 3 | Your own function that takes a parameter (REQUIRED) | GameManager.cs line 94 | Because I added a game manager that changes the state,  the player will have a more seamless experience when it moves between states (menu, playing etc).  |

| 10 | A C# event with at least one listener (REQUIRED) |  LightFlicker.cs line 39 | Because I created a script that makes the light flicker, the player gets a more realistic feeling to flames and it evokes an eery feeling when a lightbulb flickers  |

| 18 | Difficulty tuned with numbers, with before and after written down (REQUIRED) | NursePatrolCoroutine.cs line 19 | Because I added a walking speed for the nurse, the player feels the urgency of escaping the asylum |

| 4 | Your own fuction that returns and answer, like IsPlayerClose() | DetectionObserver.cs 146 | Because I made the nurse check if it can see the player, it gives the player hope that it can escape the player since it cannot always see it |

| 6 | Respawn and checkpoints | GameManager.cs line 230 | Because I built a PlayAgain method, the player isnt punished for trying and failing, but rather respawned back to the original starting position when caught |

| 7 | An enemy with readable rules | NursePatrolCoroutine.cs line 173 | Because I built an indicator above the nurses head when it is detecting the player, the player can easily read when it is safe or not |

| 8 | An animator state machine driven by a parameter | NurseAnimatorControl | Because I built an animator for the nurses, the player can see what state the nurse is in just by looking at its animations |

| 16 | A juice pass(sound, effects, animation), matched to importance | PropSFX | Because I build individual sound effects for the interactable items, the player feels like they are actually interacting with that object |

| 19 | Readability: silhouette, contrast, hitboxes that match the art | Interactor.cs line 13 | Because I built an item outline, the player can more easily see what items it can interact with |



## Check Before You Send

- [x] At least 6 rows filled in

- [x] At least 4 of them are Code items (1 to 14)

- [x] Items 3, 10 and 18 are filled in

- [x] Every Code item has a file and a line number

- [x] Brian is added to my GitHub repo

- [x] Teams message sent 