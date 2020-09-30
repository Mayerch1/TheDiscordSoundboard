from db.db_connector import SqlConnector
from model.config import Config

class ConfigDao:
    config_dao = None


    @staticmethod
    def get_instance():
        if ConfigDao.config_dao is None:
            ConfigDao.config_dao = ConfigDao()

        return ConfigDao.config_dao


    def get_config_list(self):
        """get a list of all configs

        Returns:
            list[Config]: list of all stored configs, can be empty
        """

        conn, cursor = SqlConnector.get_cursor()

        cursor.execute('''SELECT * from config where id=1''')
        result = cursor.fetchall()

        cfg_list = []
        [cfg_list.append(Config(item)) for item in result]

        return cfg_list


    def get_config_exists(self, id: int):
        """test if a id (prim key) is existing in the db

        Args:
            id (int): primary key (id) of Config

        Returns:
            bool: True if entry exists
        """

        conn, cursor = SqlConnector.get_cursor()

        cursor.execute('''SELECT exists(SELECT id from buttons WHERE id=?''', (id,))
        cfg_exists = cursor.fetchone()

        SqlConnector.close(conn)

        # convert int to bool
        return cfg_exists[0] == 1


    def get_config(self, id: int):
        """get the config with the given id
           

        Args:
            id (int): id of the config as in database

        Returns:
            Config: the requested config, None if id not exists
        """

        conn, cursor = SqlConnector.get_cursor()

        cursor.execute('''SELECT * from config WHERE id=?''', (id,))
        result = cursor.fetchone()

        SqlConnector.close(conn)

        # no foreign key
        if result is not None:
            return Config(result)
        else:
            return None


    def update_config(self, cfg: Config):
        """update the database with the current config
           NOP when config id is not in database
          
        Args:
            cfg (Config): the new config (id must be filled in and existing, otherwise NOP)
        """

        conn, cursor = SqlConnector.get_cursor()


        sql = '''UPDATE config SET bot_token=?, bot_volume=?, bot_owner_id=?, button_width=?, button_height=? WHERE id=1'''
        update_args = (cfg.bot_token, cfg.bot_volume, cfg.bot_owner_id, cfg.button_width, cfg.button_height)
        cursor.execute(sql, update_args)

        SqlConnector.commit(conn)


