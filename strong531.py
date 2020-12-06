import os
import sys

import time
from collections import namedtuple
from enum import Enum

from gspread import WorksheetNotFound
from typing import List, Tuple

import gspread
import sh
import yaml
from pathology.path import Path

strong531_planner_relative_path = 'Strong531ConsoleApp/bin/Release/netcoreapp3.1/Strong531ConsoleApp.dll'
strong531_planner = str((Path.script_dir() / strong531_planner_relative_path).resolve())

credentials_file = os.path.expanduser('~/.config/gspread/service_account.json')

# Full URL: https://docs.google.com/spreadsheets/d/1wKAxfUFhAzgnrGoZRzptIduTxCRdi_5-r9ZO5XDi40w
strong531_key = '1wKAxfUFhAzgnrGoZRzptIduTxCRdi_5-r9ZO5XDi40w'

Color = namedtuple('Color', ['R', 'G', 'B'])


class Alignment(Enum):
    LEFT = 'LEFT'
    CENTER = 'CENTER'
    RIGHT = 'RIGHT'


# If dry_run is True just calculate the values, but don't update values in the worksheets
dry_run = False

class CellFormat:
    def __init__(self,
                 bg_color: Color = Color(255, 255, 255),
                 text_color: Color = Color(0, 0, 0),
                 font_size: int = 12,
                 bold: bool = False,
                 alignment: Alignment = Alignment.CENTER):
        self.bg_color = bg_color
        self.text_color = text_color
        self.font_size = font_size
        self.bold = bold
        self.alignment = alignment

    def as_dict(self):
        return dict(
            backgroundColor=dict(
                red=self.bg_color.R,
                green=self.bg_color.G,
                blue=self.bg_color.B),
            horizontalAlignment=self.alignment.value,
            textFormat=dict(
                foregroundColor=dict(
                    red=self.text_color.R,
                    green=self.text_color.G,
                    blue=self.text_color.B),
                fontSize=self.font_size,
                bold=self.bold)
        )


class GoogleWorksheet:
    def __init__(self, ws):
        self.ws = ws

    @staticmethod
    def cell2label(row, col):
        return chr(ord('A') - 1 + col) + str(row)

    def update_cell(self, row, col, value):
        self.ws.update_cell(row, col, value)

    def update_cells(self, start, end, values):
        range_label = f'{self.cell2label(*start)}:{self.cell2label(*end)}'
        self.ws.update(range_label, [[v] for v in values])

    def format(self, cells: List[Tuple], format: CellFormat):
        for cell in cells:
            cf = format.as_dict()
            label = self.index2label(cell[0], cell[1])
            self.ws.format(label, cf)


class GoogleSheetManager:
    def __init__(self, spreadsheet_key, template_sheet):
        if not os.path.isfile(credentials_file):
            raise RuntimeError('Missing credentials file. RTFM (README.md)')

        self.gc = gspread.service_account()
        self.spreadsheet = self.gc.open_by_key(spreadsheet_key)

        # find template worksheet
        self.template_id = None
        for s in self.sheets:
            if s.title == template_sheet:
                self.template_id = s.id
                break
        if self.template_id is None:
            raise RuntimeError('Missing template sheet: ' + template_sheet)

    def get_worksheet(self, title):
        return GoogleWorksheet(self.spreadsheet.worksheet(title))

    def get_titles(self):
        return [s.title for s in self.sheets]

    @property
    def sheets(self):
        return self.spreadsheet.worksheets()

    def add_worksheet(self, title):
        """Appends a new worksheet, which is a a duplicate of the template worksheet

        If there is already a worksheet with this title just return it (idempotent)
        """
        try:
            # Check if a worksheet with this title already exists and bail out
            self.spreadsheet.worksheet(title)
            return
        except WorksheetNotFound:
            pass

        index = len(self.sheets)
        ws = self.spreadsheet.duplicate_sheet(self.template_id, insert_sheet_index=index, new_sheet_name=title)
        return ws


class Strong531:
    def __init__(self, cycles):
        self.plan = self.generate_plan(cycles)
        self.gsm = GoogleSheetManager(strong531_key, 'Template')

    @staticmethod
    def generate_plan(cycles):
        if not os.path.isfile(strong531_planner):
            raise RuntimeError('Missing planner. Build the Strong531ConsoleApp in Release mode')

        plan = sh.dotnet(strong531_planner, cycles).stdout.decode('utf-8')
        return yaml.safe_load(plan)

    def dump_plan(self):
        """ """
        # for user, user_plan in self.plan.items():
        #     print(user)
        #     print('-' * 20)
        #     pp(user_plan)
        #     print()

    def get_last_cycle(self):
        titles = self.gsm.get_titles()
        cycles = [int(t.split()[-1]) for t in titles if t.startswith('Cycle')]

        return 0 if not cycles else max(cycles)

    @staticmethod
    def find_col(week, user):
        """Find the column of a user in a given week
        """
        users = dict(enumerate('Liat Guy Saar Gigi'.split()))
        weeks = dict(enumerate('OfFive OfThree OfOne'.split()))

        users = {v: k for k, v in users.items()}
        weeks = {v: k for k, v in weeks.items()}

        col = 2 + weeks[week] * 9 + users[user] * 2
        return col

    @staticmethod
    def find_row(lift):
        """Find the first row of a particular lift
        """
        lifts = dict(enumerate(('Squat Press Deadlift Bench'.split())))
        lifts = {v: k for k, v in lifts.items()}

        row = 5 + lifts[lift] * 7
        return row

    def populate(self):
        cycle_count = len(list(self.plan.values())[0]['Cycles'])
        last_cycle = self.get_last_cycle()
        titles = [f'Cycle {last_cycle + 1 + i}' for i in range(cycle_count)]
        for title in titles:
            self.gsm.add_worksheet(title)

        for user, user_plan in self.plan.items():
            for i, cycle in enumerate(user_plan['Cycles']):
                ws = self.gsm.get_worksheet(titles[i])
                for week_name, week in cycle['Weeks'].items():
                    if week_name == 'Deload':
                        continue
                    col = self.find_col(week_name, user)
                    for lift_name, lift in week.items():
                        if not lift:
                            continue
                        row = self.find_row(lift_name)
                        start = (row, col)
                        end = (row + 5, col)
                        values = [lift[j]['Weight'] for j in range(6)]
                        ws.update_cells(start, end, values)
                        # Must sleep because  of Google API rate limit of 100 requests per 100 seconds per user
                        # See https://developers.google.com/sheets/api/limits
                        time.sleep(1)


def main():
    """ """
    cycles = sys.argv[1] if len(sys.argv) > 1 else '2'
    s = Strong531(cycles)
    s.dump_plan()
    s.populate()


if __name__ == '__main__':
    main()
