import os
import gspread
import sh
import yaml
from pathology.path import Path
from pprint import pprint as pp

strong531_planner_relative_path = 'Strong531ConsoleApp/bin/Release/netcoreapp3.1/Strong531ConsoleApp.dll'
strong531_planner = str((Path.script_dir() / strong531_planner_relative_path).resolve())

credentials_file = os.path.expanduser('~/.config/gspread/service_account.json')

# Full URL: https://docs.google.com/spreadsheets/d/1wKAxfUFhAzgnrGoZRzptIduTxCRdi_5-r9ZO5XDi40w
strong531_key = '1wKAxfUFhAzgnrGoZRzptIduTxCRdi_5-r9ZO5XDi40w'


class GoogleSheetManager:
    def __init__(self, spreadsheet_key):
        if not os.path.isfile(strong531_planner):
            raise RuntimeError('Missing planner. Build the Strong531ConsoleApp in Release mode')

        if not os.path.isfile(credentials_file):
            raise RuntimeError('Missing credentials file. RTFM (README.md)')

        self.gc = gspread.service_account()
        self.spreadsheet = self.gc.open_by_key(spreadsheet_key)

    def add_worksheet(self, name):
        """ """


class Strong531:
    def __init__(self):
        self.plan = self.generate_plan()
        # self.gsm = GoogleSheetManager(strong531_key)

    def generate_plan(self):
        plan = sh.dotnet(strong531_planner).stdout.decode('utf-8')
        return yaml.safe_load(plan)

    def dump_plan(self):
        for user, user_plan in self.plan.items():
            print(user)
            print('-' * 20)
            pp(user_plan)
            print()


def main():
    """ """
    s = Strong531()
    s.dump_plan()


if __name__ == '__main__':
    main()
