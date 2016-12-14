﻿export const TestForm = {
    'model': {
        'MainColumnMain': {
            'ID': '001',
            'Name': 'John Doe',
            'Phone': '7777777777',
            'Address': 'Scruff McGruff',
            'City': 'Chicago',
            'State': 'Illinois',
            'Zip': '60652'
        }
    },
    'interface': {
        'value': `
            <d4-interface>
              <d4-column name="RootEditColumn">
                <d4-column name="Element1">
                  <d4-numerictextbox name="MainColumnMain.ID" nilifblank="True"></d4-numerictextbox>
                  <d4-textbox name="MainColumnMain.Name" title="Customized Name"></d4-textbox>
                  <d4-textbox name="MainColumnMain.Phone" title="Customized Phone"></d4-textbox>
                  <d4-textbox name="MainColumnMain.Address" title="Customized Address"></d4-textbox>
                  <d4-textbox name="MainColumnMain.City" title="Customized City"></d4-textbox>
                  <d4-textbox name="MainColumnMain.State" title="Customized State"></d4-textbox>
                  <d4-textbox name="MainColumnMain.Zip" title="Customized Zip"></d4-textbox>
                </d4-column>
              </d4-column>
            </d4-interface>
        `
    }
};