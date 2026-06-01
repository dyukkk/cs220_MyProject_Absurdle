# Absurdle

A command-line word-guessing game in F#, inspired by Wordle but adversarial:
instead of picking a secret word up front, the game keeps a set of all
possible answers and, after each guess, returns whatever feedback keeps that
set as large as possible. So it's basically Wordle that cheats, but only
in ways consistent with the feedback it has already given you.

Written in F# on .NET 10.

## Running it

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download). Check
with `dotnet --version` — it should print something starting with `10`.

From the repo root:

```
dotnet run
```

That's it. The first run takes a minute while .NET sets things up; after
that it's fast.

Options:

```
dotnet run -- --seed 42         # fixes the RNG (only affects :reveal)
dotnet run -- --words other.txt # use a different word list
```

## How to play

Type a 5-letter word and hit Enter. Each letter is colored:

- green = right letter, right position
- yellow = right letter, wrong position
- gray = letter isn't in the answer

The game prints how many possible answers are still consistent below your
guess. There's no guess limit. You win when all 5 letters turn green.

At any prompt you can also type `:quit` to exit or `:reveal` to give up
and see one possible answer.

## What's where

```
Absurdle.fsproj           project file
words.txt                 1853 common 5-letter English words
requirements.pdf          the requirements doc I wrote for the proposal
Absurdle/
  Feedback.fs   wordle-style color feedback (handles repeated letters)
  Adversary.fs  the adversarial pattern-picking - the actual core idea
  Render.fs     ANSI color output
  Words.fs      loads + filters the word list
  Game.fs       per-round game loop, input handling
  Program.fs    entry point, CLI args, play-again loop
```

The interesting file is `Adversary.fs`. The whole game is "group the
candidate words by what feedback pattern each would give against the
player's guess, then keep the biggest group." Tie-breaking (fewer greens,
then fewer yellows, then lex order) is in there too, that part is
literally R6 from the requirements doc.

## Changes from the proposal

None. The implementation matches the requirements document.

## LLM usage

Per section 7 of the project spec, I have to declare how I used an LLM
(Claude). Here's what it actually did and didn't do.

**Used it for:**

- Brainstorming game ideas. I went through a few directions (RL-trained
  agents, MCTS, Connect Four, Dots and Boxes) before settling on
  Absurdle because it's actually fun to play and the adversarial
  algorithm is interesting on its own.
- F# things I didn't know off the top of my head: how `Array.groupBy`
  works, that F# tuples compare lex (which is why the tie-break code
  in `Adversary.fs` is so short), and ANSI escape codes that render
  the same way on Linux and Windows terminals.
- Drafting the requirements PDF and this README from my notes.
- The two-pass duplicate-letter algorithm in `Feedback.compute`, I had
  the idea but kept getting the second pass wrong by hand, and walking
  through it with the LLM helped me see why the "mark used positions"
  trick is necessary.

**What I did myself:**

- Decided what game to make and wrote the requirements doc (R1–R16,
  the tie-break rules in R6).
- Designed the module split.
- Wrote the adversarial logic in `Adversary.fs` and the game loop in
  `Game.fs`.

**Where the LLM got things wrong:**

- It first tried to group candidates using F# arrays as keys, which
  silently doesn't work, F# arrays don't have structural equality, so
  every array ends up in its own group. Switched to encoding the
  pattern as a string instead.
- The first version of `validate` had three error messages, but R4
  only specifies two. Looked fine until I checked it against the
  requirements line by line.

## License

MIT — see LICENSE.
