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
        conn, cursor = SqlConnector.get_cursor()

        cursor.execute('''SELECT * from config where id=1''')
        result = cursor.fetchone()

        cfg_list = []
        [cfg_list.append(Config(item)) for item in result]

        return cfg_list


    def get_config_exists(self, id: int):

        conn, cursor = SqlConnector.get_cursor()

        cursor.execute('''SELECT exists(SELECT id from buttons WHERE id=?''', (id,))
        cfg_exists = cursor.fetchone()

        SqlConnector.close(conn)

        # convert int to bool
        return cfg_exists[0] == 1


    def get_config(self, id: int):

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

        conn, cursor = SqlConnector.get_cursor()


        sql = '''UPDATE config SET bot_token=?, bot_volume=?, bot_owner_id=?, button_width=?, button_height=? WHERE id=1'''
        update_args = (cfg.bot_token, cfg.bot_volume, cfg.bot_owner_id, cfg.button_width, cfg.button_height)
        cursor.execute(sql, update_args)

        SqlConnector.commit(conn)


