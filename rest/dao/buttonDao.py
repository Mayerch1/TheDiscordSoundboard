from db.db_connector import SqlConnector

from dao.trackDataDao import TrackDataDao
from model.button import Button


class ButtonDao:
    button_dao = None


    @staticmethod
    def get_instance():
        if ButtonDao.button_dao is None:
            ButtonDao.button_dao = ButtonDao()

        return ButtonDao.button_dao




    def get_button_list(self):
        """get a list of all buttons
           foreign keys are not resolved

        Returns:
            list[Button]: list of all stored buttons, can be empty
        """

        conn, cursor = SqlConnector.get_cursor()

        cursor.execute('''SELECT * FROM buttons''')
        result = cursor.fetchall()

        SqlConnector.close(conn)

        # convert all into python-typed objects
        btn_list = []
        [btn_list.append(Button(item)) for item in result]

        return btn_list


    def get_button_exists(self, id: int):
        """test if a id (prim key) is existing in the db

        Args:
            id (int): primary key (id) of button

        Returns:
            bool: True if entry exists
        """

        conn, cursor = SqlConnector.get_cursor()

        cursor.execute('''SELECT exists(SELECT id from buttons WHERE id=?)''', (id,))
        btn_exists = cursor.fetchone()

        SqlConnector.close(conn)

        # convert int to bool
        return btn_exists[0] == 1




    def get_button(self, id: int):
        """get the button with the given id
           foreign keys are resolved

        Args:
            id (int): id of the button as in database

        Returns:
            Button: the requested button, None if id not exists
        """
        conn, cursor = SqlConnector.get_cursor()

        cursor.execute('''SELECT * FROM buttons WHERE id=?''', (id,))
        button = cursor.fetchone()

        SqlConnector.close(conn)

        if button is not None:
            btn = Button()
            btn.parse_from_tuple(button)

            # resolve the foreign key
            if btn.track_id is not None:
                btn.track = TrackDataDao.get_instance().get_track(btn.track_id)

            # override the tuple for easier program flow
            button = btn


        return button





    def add_button(self, btn: Button):
        """add a new button to the db
           if track_id is in db, no further action
           if track_id not in db, but track specified, add track to db
           if track_id not in db, but track missing, return None (FAIL)

           returns the button as saved in db (with resolved track (when track_id existing))

        Args:
            btn (Button): the button to be saved, track_id must be existing, None (or match with provided track member)

        Returns:
            [Button]: button with resolved track (when possible), None on error
        """
        
        
        # check if foreign key exists in db, when it does no further action
        if btn.track_id is not None:

            track_exists = TrackDataDao.get_instance().get_track_exists(btn.track_id)

            # if it does not exist, check if track member is specified
            if not track_exists:
                return None


        # insert new track into the db
        if btn.track_id is None and btn.track:
            btn.track = TrackDataDao.get_instance().add_track(btn.track)
            btn.track_id = btn.track.id

        # resolve foreign key
        elif btn.track_id:
            btn.track = TrackDataDao.get_instance().get_track(btn.track_id)


        conn, cursor = SqlConnector.get_cursor()

        sql = ''' INSERT INTO buttons (position, nick_name, is_earrape, is_loop, track_id) VALUES (?, ?, ?, ?, ?) '''
        btn_args = (btn.position, btn.nick_name, btn.is_earrape, btn.is_loop, btn.track_id)
        cursor.execute(sql, btn_args)

        btn_entry = cursor.execute('SELECT last_insert_rowid()')
        btn_id = btn_entry.fetchone()

        btn.id = btn_id[0]

        SqlConnector.commit(conn)

        return btn


    def delete_button(self, id: int):
        """deletes the button with the given id
           sqlite executes NOP when id not existing

        Args:
            id (int): id of the button
        """

        conn, cursor = SqlConnector.get_cursor()
        cursor.execute('''DELETE FROM buttons WHERE id=?''', (id,))
        SqlConnector.commit(conn)



    def update_button(self, btn: Button):
        """update the database with the current button
           the referenced track is updated aswell
           if the referenced track changed, it is only updated when the foreign key-id matches the given track id

        Args:
            btn (Button): the new button (id must be filled in and existing, otherwise NOP)
        """

        conn, cursor = SqlConnector.get_cursor()

        sql = '''UPDATE buttons SET position=?, nick_name=?, is_earrape=?, is_loop=?, track_id=? WHERE id=?'''
        update_args = (btn.position, btn.nick_name, btn.is_earrape, btn.is_loop, btn.track_id, btn.id)
        cursor.execute(sql, update_args)

        SqlConnector.commit(conn)


        # next check if the contained trackData was modified
        # do nothing if foreign key id does not match track id
        if btn.track and btn.track.id == btn.track_id:
            TrackDataDao.get_instance().update_track(btn.track)




