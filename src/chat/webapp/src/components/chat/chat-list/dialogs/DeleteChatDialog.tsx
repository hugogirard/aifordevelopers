import { Button } from '@fluentui/react-button';
import { Tooltip, makeStyles } from '@fluentui/react-components';
import {
    Dialog,
    DialogActions,
    DialogBody,
    DialogContent,
    DialogSurface,
    DialogTitle,
    DialogTrigger,
} from '@fluentui/react-dialog';
import { useChat } from '../../../../libs/hooks';
import { getFriendlyChatName } from '../../../../libs/hooks/useChat';
import { useAppSelector } from '../../../../redux/app/hooks';
import { Delete16 } from '../../../shared/BundledIcons';
import { useLanguageContext } from "../../../../language/languageContext";

const useClasses = makeStyles({
    root: {
        width: '450px',
    },
    actions: {
        paddingTop: '10%',
    },
});

interface IEditChatNameProps {
    chatId: string;
}

export const DeleteChatDialog: React.FC<IEditChatNameProps> = ({ chatId }) => {
    const classes = useClasses();
    const { t } = useLanguageContext();
    const chat = useChat();

    const { conversations } = useAppSelector((state) => state.conversations);
    const chatName = getFriendlyChatName(conversations[chatId]);

    const onDeleteChat = () => {
        void chat.deleteChat(chatId);
    };

    return (
        <Dialog modalType="alert">
            <DialogTrigger>
                <Tooltip content={t("DeleteChatSession")} relationship="label">
                    <Button icon={<Delete16 />} appearance="transparent" aria-label="Edit" />
                </Tooltip>
            </DialogTrigger>
            <DialogSurface className={classes.root}>
                <DialogBody>
                    <DialogTitle>{t("DeleteChatConfirm")}: {chatName}?</DialogTitle>
                    <DialogContent>
                        {t("DeleteChatInfo")}
                    </DialogContent>
                    <DialogActions className={classes.actions}>
                        <DialogTrigger action="close" disableButtonEnhancement>
                            <Button appearance="secondary">{t("Cancel")}</Button>
                        </DialogTrigger>
                        <DialogTrigger action="close" disableButtonEnhancement>
                            <Button appearance="primary" onClick={onDeleteChat}>
                                {t("Delete")}
                            </Button>
                        </DialogTrigger>
                    </DialogActions>
                </DialogBody>
            </DialogSurface>
        </Dialog>
    );
};
