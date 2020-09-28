import sqlite3
import os
from sqlite3 import Error




class SqlConnector:
    connection = None
    cursor = None


    @staticmethod
    def init_db():
        conn = sqlite3.connect('test.db')
        curs = conn.cursor()

        # execute the creatDB script
        with open('./db/loadDatabase.sql', 'r') as file:
            file_content = file.readlines()
            file_content = list(filter(lambda l: not l.startswith('--'), file_content))
            file_content = ' '.join(file_content)

        # cut into all commands (execute can only do one command at time)
        # split would cut out the delimiter
        delimiter = ';'
        sql_scripts = file_content.split('--')
        sql_scripts = [match + delimiter for match in file_content.split(delimiter) if match]

        #for script in sql_scripts:
        #    curs.execute(script)
        #curs.execute(sql_scripts[1])
        curs.executescript(file_content)

        conn.commit()

        conn.close()

    @staticmethod
    def get_cursor():
        if not SqlConnector.cursor:
            SqlConnector._open_connection()

        if not SqlConnector.cursor:
            raise Exception('dB error')

        return SqlConnector.cursor


    @staticmethod
    def commit():
        if SqlConnector.connection:

            SqlConnector.connection.commit()
            SqlConnector.close()



    @staticmethod
    def close():
        if SqlConnector.connection:

            SqlConnector.connection.close()

            SqlConnector.connection = None
            SqlConnector.cursor = None



    @staticmethod
    def _open_connection(db_file = 'test.db'):
        
        try:
            SqlConnector.connection = sqlite3.connect(db_file)
        except Error as e:
            print(e)
            SqlConnector.connection = None
            return False
        else:
            SqlConnector.cursor = SqlConnector.connection.cursor()
            return True