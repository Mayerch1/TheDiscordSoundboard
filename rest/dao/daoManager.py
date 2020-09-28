

class DaoManager:
    dao_manager = None
    entity_manager = None



    @staticmethod
    def get_instance():
        if DaoManager.dao_manager is None:
            DaoManager.dao_manager = DaoManager()

        return DaoManager.dao_manager


    @staticmethod
    def get_entity_manager(self):

        pass
