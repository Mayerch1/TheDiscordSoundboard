from db.db_connector import SqlConnector

from model.trackData import TrackData


class TrackDataDao:
    track_dao = None


    @staticmethod
    def get_instance():
        if TrackDataDao.track_dao is None:
            TrackDataDao.track_dao = TrackDataDao()

        return TrackDataDao.track_dao



    def get_track_list(self):
        """get a list of all trackDatas
           foreign keys are not resolved

        Returns:
            list[TrackData]: list of all stored tracks, can be empty
        """

        cursor = SqlConnector.get_cursor()

        cursor.execute('''SELECT * FROM trackdatas''')
        result = cursor.fetchall()

        # convert all into python-typed objects
        trk_list = []
        [trk_list.append(TrackData(item)) for item in result]

        return trk_list



    def get_track_exists(self, id: int):
        """test if a id (prim key) is existing in the db

        Args:
            id (int): primary key (id) of track

        Returns:
            bool: True if entry exists
        """

        cursor = SqlConnector.get_cursor()

        cursor.execute('''SELECT exists(SELECT id from trackdatas WHERE id=?)''', (id,))
        track_exists = cursor.fetchone()

        SqlConnector.close()

        # convert int to bool
        return track_exists[0] == 1




    def get_track(self, id: int):
        """get the trackData with the given id
           foreign keys are resolved

        Args:
            id (int): id of the track as in database

        Returns:
            TrackData: the requested track, None if id not exists
        """
        cursor = SqlConnector.get_cursor()

        cursor.execute('''SELECT * FROM trackdatas WHERE id=?''', (id,))
        track = cursor.fetchone()

        SqlConnector.close()

        if track is not None:
            trk = TrackData()
            trk.parse_from_tuple(track)

            # override the tuple for easier program flow
            track = trk


        return track



    def add_track(self, track: TrackData):
        """add new track to the db
           ignores id when already specified
           (might potentially add clone to db when id already saved)

        Args:
            track (TrackData): the track to be saved

        Returns:
            [TrackData]: track as saved into db
        """

        cursor = SqlConnector.get_cursor()

        sql = '''INSERT INTO trackdatas (name, local_file, uri, image_uri, description, author, album, genre, duration) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)'''
        track_args = (track.name, track.local_file, track.uri, track.image_uri, track.description, track.author, track.album, track.genre, track.duration)
        cursor.execute(sql, track_args)

        track_entry = cursor.execute('SELECT last_insert_rowid()')
        track_id = track_entry.fetchone()

        track.id = track_id[0]


        SqlConnector.commit()


        return track



    def delete_track(self, id: int):
        """deletes the track with the given id
           sqlite executes NOP when id not existing

        Args:
            id (int): id of the track
        """

        cursor = SqlConnector.get_cursor()
        cursor.execute('''DELETE FROM trackdatas WHERE id=?''', (id,))
        SqlConnector.commit() 


    def update_track(self, trk: TrackData):
        """update the database with the curren trackData
           sqlite does nothing if the trk did not change

        Args:
            trk (TrackData): the new track (id must be filled in and existing, otherwise NOP)
        """

        cursor = SqlConnector.get_cursor()

        sql = '''UPDATE trackdatas SET name=?, local_file=?, uri=?, image_uri=?, description=?, author=?, album=?, genre=?, duration=? WHERE id=?'''
        update_args = (trk.name, trk.local_file, trk.uri, trk.image_uri, trk.description, trk.author, trk.album, trk.genre, trk.duration, trk.id)
        cursor.execute(sql, update_args)

        SqlConnector.commit()