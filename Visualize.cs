using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Console;

OutputEncoding = Encoding.UTF8;
CursorVisible = false;

int N = 5;
int boardSize = N;
int Row = 0;
int[] colspan = Enumerable.Repeat (-1, N).ToArray ();

bool[] clOccupancy = new bool[boardSize];
bool[] d1Occupancy = new bool[(2 * boardSize) - 1];
bool[] d2Occupancy = new bool[(2 * boardSize) - 1];
int total = 0;

List<(int[] board, int cR, int score)> States = new ();

const string TOPPATTERN = "┌┬┐";
const string MIDPATTERN = "├┼┤";
const string BOTPATTERN = "└┴┘";
const string HORILINE = "────";
const string VERTLINE = "│";

void DrawBoard (int[] map, int cR, int sc) {
   SetCursorPosition (0, 0); 
   PrintOuter (TOPPATTERN);

   for (int r = 0; r < boardSize; r++) {
      PrintInternal (r, map);
      if (r != boardSize - 1) {
         PrintOuter (MIDPATTERN);
      }
   }

   PrintOuter (BOTPATTERN);
   WriteLine ($"\nSolutions found so far: {sc}");
   WriteLine ("\nControls: [→] Next | [←] Previous | [ESC] Exit");

   void PrintOuter (string pattern) =>
      WriteLine ($"{pattern[0]}{string.Join (pattern[1], Enumerable.Repeat (HORILINE, boardSize))}{pattern[2]}");

   void PrintInternal (int rowId, int[] map) {
      Write (VERTLINE);
      for (int c = 0; c < boardSize; c++) {
         if (c == map[rowId] && rowId < cR) {
            ForegroundColor = ConsoleColor.Green; Write (" ♕  "); ResetColor ();
         } else if (c == map[rowId]) {
            ForegroundColor = ConsoleColor.Red; Write (" ♕  "); ResetColor ();
         } else {
            Write ("    ");
         }
         Write (VERTLINE);
      }
      WriteLine ();
   }
}

// to mark the diagonals and columns as Occupied/Unoccupied
void editLookUpState (int r, int c, bool State) {
   clOccupancy[c] = State;
   d1Occupancy[r + c] = State;
   d2Occupancy[r - c + (boardSize - 1)] = State;
}

// Checks if a square is under attack
bool IsValidx (int r, int c) {
   if (clOccupancy[c]) return false;
   if (d1Occupancy[r + c]) return false;
   if (d2Occupancy[r - c + (boardSize - 1)]) return false;
   return true;
}

while (Row >= 0 && Row < boardSize) {
   bool placed = false;
   for (int Col = colspan[Row] + 1; Col < boardSize; Col++) {
      colspan[Row] = Col;
      // Make it Red ( un-confirmed )
      States.Add (((int[])colspan.Clone (), Row, total));
      if (IsValidx (Row, Col)) {
         // Make it Green ( confirmed )
         States.Add (((int[])colspan.Clone (), Row + 1, total));
         editLookUpState (Row, Col, true);
         colspan[Row] = Col;
         placed = true;
         Row++;
         break;
      }
   }

   // If a valid configuration is found for all rows
   if (Row == boardSize) {
      total++;
      States.Add (((int[])colspan.Clone (), Row, total));
      Row--;
      editLookUpState (Row, colspan[Row], false);
   }

   // If a queen cannot be placed in the current row
   if (!placed) {
      colspan[Row] = -1;
      // the before line removes the queen from the unplaced 
      States.Add (((int[])colspan.Clone (), Row, total));
      Row--;
      if (Row >= 0) {
         editLookUpState (Row, colspan[Row], false);
      }
   }
}

int current = 0;

for (; ; ) {
   var state = States[current];
   DrawBoard (state.board, state.cR, state.score);

   switch (ReadKey (true).Key) {
      case ConsoleKey.RightArrow when current < States.Count - 1:
         current++;
         break;

      case ConsoleKey.LeftArrow when current > 0:
         current--;
         break;

      case ConsoleKey.Escape:
         CursorVisible = true; 
         return;
   }
}
