// Copyright (c) Microsoft. All rights reserved.

import {
    AvatarProps,
    Persona,
    Text,
    ToggleButton,
    Tooltip,
    makeStyles,
    mergeClasses,
    shorthands,
    Button
} from '@fluentui/react-components';
import { ChevronDown20Regular, ChevronUp20Regular, ThumbDislikeFilled, ThumbLikeFilled, Play16Regular } from '@fluentui/react-icons';
import React, { useState } from 'react';
import { useChat } from '../../../libs/hooks/useChat';
import { AuthorRoles, ChatMessageType, IChatMessage, UserFeedback } from '../../../libs/models/ChatMessage';
import { useAppSelector } from '../../../redux/app/hooks';
import { RootState } from '../../../redux/app/store';
import { DefaultChatUser, FeatureKeys } from '../../../redux/features/app/AppState';
import { Breakpoints, customTokens } from '../../../styles';
import { timestampToDateString } from '../../utils/TextUtils';
import { PlanViewer } from '../plan-viewer/PlanViewer';
import { PromptDialog } from '../prompt-dialog/PromptDialog';
import { TypingIndicator } from '../typing-indicator/TypingIndicator';
import * as utils from './../../utils/TextUtils';
import { ChatHistoryDocumentContent } from './ChatHistoryDocumentContent';
import { ChatHistoryTextContent } from './ChatHistoryTextContent';
import { CitationCards } from './CitationCards';
import { UserFeedbackActions } from './UserFeedbackActions';
import { useMsal } from '@azure/msal-react';
import { AuthHelper } from '../../../libs/auth/AuthHelper';
import { SpeechService } from '../../../libs/services/SpeechService';
import * as speechSdk from 'microsoft-cognitiveservices-speech-sdk';
import { useLanguageContext } from "../../../language/languageContext";

const useClasses = makeStyles({
    root: {
        display: 'flex',
        flexDirection: 'row',
        maxWidth: '75%',
        minWidth: '24em',
        ...shorthands.borderRadius(customTokens.borderRadiusMedium),
        ...Breakpoints.small({
            maxWidth: '100%',
        }),
        ...shorthands.gap(customTokens.spacingHorizontalXS),
    },
    debug: {
        position: 'absolute',
        top: '-4px',
        right: '-4px',
    },
    alignEnd: {
        alignSelf: 'flex-end',
    },
    persona: {
        paddingTop: customTokens.spacingVerticalS,
    },
    item: {
        backgroundColor: customTokens.colorNeutralBackground1,
        ...shorthands.borderRadius(customTokens.borderRadiusMedium),
        ...shorthands.padding(customTokens.spacingVerticalXS, customTokens.spacingHorizontalS),
    },
    me: {
        backgroundColor: customTokens.colorMeBackground,
        width: '100%',
    },
    time: {
        color: customTokens.colorNeutralForeground3,
        fontSize: customTokens.fontSizeBase200,
        fontWeight: 400,
    },
    header: {
        position: 'relative',
        display: 'flex',
        flexDirection: 'row',
        ...shorthands.gap(customTokens.spacingHorizontalL),
    },
    canvas: {
        width: '100%',
        textAlign: 'center',
    },
    image: {
        maxWidth: '250px',
    },
    blur: {
        filter: 'blur(5px)',
    },
    controls: {
        display: 'flex',
        flexDirection: 'row',
        marginTop: customTokens.spacingVerticalS,
        marginBottom: customTokens.spacingVerticalS,
        ...shorthands.gap(customTokens.spacingHorizontalL),
    },
    citationButton: {
        marginRight: 'auto',
    },
    rlhf: {
        marginLeft: 'auto',
    },
    playButton: {
        ...shorthands.padding(0),
        ...shorthands.margin(0),
        minWidth: 'auto'
    },    
});

interface ChatHistoryItemProps {
    message: IChatMessage;
    messageIndex: number;
}

export const ChatHistoryItem: React.FC<ChatHistoryItemProps> = ({ message, messageIndex }) => {
    const classes = useClasses();
    const { t } = useLanguageContext();
    const chat = useChat();
    const { instance, inProgress } = useMsal();    

    const { conversations, selectedId } = useAppSelector((state: RootState) => state.conversations);
    const { activeUserInfo, features } = useAppSelector((state: RootState) => state.app);
    const [showCitationCards, setShowCitationCards] = useState(false);

    const isDefaultUser = message.userId === DefaultChatUser.id;
    const isMe = isDefaultUser || (message.authorRole === AuthorRoles.User && message.userId === activeUserInfo?.id);
    const isBot = message.authorRole === AuthorRoles.Bot;
    const user = isDefaultUser
        ? DefaultChatUser
        : chat.getChatUserById(message.userName, selectedId, conversations[selectedId].users);
    const fullName = user?.fullName ?? message.userName;

    const avatar: AvatarProps = isBot
        ? { image: { src: conversations[selectedId].botProfilePicture } }
        : isDefaultUser
          ? { idForColor: selectedId, color: 'colorful' }
          : { name: fullName, color: 'colorful' };

    let content: JSX.Element;
    if (isBot && message.type === ChatMessageType.Plan) {
        content = <PlanViewer message={message} messageIndex={messageIndex} />;
    } else if (message.type === ChatMessageType.Document) {
        content = <ChatHistoryDocumentContent isMe={isMe} message={message} />;
    } else {
        content =
            isBot && message.content.length === 0 ? <TypingIndicator /> : <ChatHistoryTextContent message={message} />;
    }

    // TODO: [Issue #42] Persistent RLHF, hook up to model
    // Currently for demonstration purposes only, no feedback is actually sent to kernel / model
    const showShowRLHFMessage =
        features[FeatureKeys.RLHF].enabled &&
        message.userFeedback === UserFeedback.Requested &&
        messageIndex === conversations[selectedId].messages.length - 1 &&
        message.userId === 'Bot';

    const messageCitations = message.citations ?? [];
    const showMessageCitation = messageCitations.length > 0;
    const showExtra = showMessageCitation || showShowRLHFMessage || showCitationCards;

    async function playText(text: string) {
        const speechService = new SpeechService();
        const response = await speechService.getSpeechTokenAsync(
            await AuthHelper.getSKaaSAccessToken(instance, inProgress),
        );
        if (response.isSuccess) {
            const synthesizer = speechService.getSpeechSynthesizerAsyncWithValidKey(response);
            synthesizer?.speakTextAsync(text,   function (result) {
                if (result.reason === speechSdk.ResultReason.SynthesizingAudioCompleted) {
                  console.log("Speech synthesis finished.");
                } else {
                  console.error("Speech synthesis canceled, " + result.errorDetails +
                      "\nDid you set the speech resource key and region values?");
                }
                synthesizer.close();
              },
              function (err) {
                console.trace("Speech synthesis faield: " + err);
                synthesizer.close();
              });
        }
        console.log('Done');
    }    

    return (
        <div
            className={isMe ? mergeClasses(classes.root, classes.alignEnd) : classes.root}
            // The following data attributes are needed for CI and testing
            data-testid={`chat-history-item-${messageIndex}`}
            data-username={fullName}
            data-content={utils.formatChatTextContent(message.content)}
        >
            {
                <Persona
                    className={classes.persona}
                    avatar={avatar}
                    presence={
                        !features[FeatureKeys.SimplifiedExperience].enabled && !isMe
                            ? { status: 'available' }
                            : undefined
                    }
                />
            }
            <div className={isMe ? mergeClasses(classes.item, classes.me) : classes.item}>
                <div className={classes.header}>
                    {!isMe && <Text weight="semibold">{fullName}</Text>}
                    <Text className={classes.time}>{timestampToDateString(message.timestamp, true)}</Text>
                    {isBot && <PromptDialog message={message} />}
                    {isBot && <Tooltip content={t("Play")} relationship='label'><Button className={classes.playButton} icon={<Play16Regular />} appearance="transparent" onClick={ () => { playText(message.content).catch((error: Error) => { console.error(error) })}} /></Tooltip> }
                </div>
                {content}
                {showExtra && (
                    <div className={classes.controls}>
                        {showMessageCitation && (
                            <ToggleButton
                                appearance="subtle"
                                checked={showCitationCards}
                                className={classes.citationButton}
                                icon={showCitationCards ? <ChevronUp20Regular /> : <ChevronDown20Regular />}
                                iconPosition="after"
                                onClick={() => {
                                    setShowCitationCards(!showCitationCards);
                                }}
                                size="small"
                            >
                                {`${messageCitations.length} ${
                                    messageCitations.length === 1 ? t("Citation") : t("Citations")
                                }`}
                            </ToggleButton>
                        )}
                        {showShowRLHFMessage && (
                            <div className={classes.rlhf}>{<UserFeedbackActions messageIndex={messageIndex} />}</div>
                        )}
                        {showCitationCards && <CitationCards message={message} />}
                    </div>
                )}
            </div>
            {features[FeatureKeys.RLHF].enabled && message.userFeedback === UserFeedback.Positive && (
                <ThumbLikeFilled color="gray" />
            )}
            {features[FeatureKeys.RLHF].enabled && message.userFeedback === UserFeedback.Negative && (
                <ThumbDislikeFilled color="gray" />
            )}
        </div>
    );
};
