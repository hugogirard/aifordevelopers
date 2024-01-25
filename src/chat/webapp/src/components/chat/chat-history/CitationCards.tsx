// Copyright (c) Microsoft. All rights reserved.

import {
    Badge,
    Caption1,
    Card,
    CardHeader,
    makeStyles,
    shorthands
} from '@fluentui/react-components';
import { IChatMessage } from '../../../libs/models/ChatMessage';
import { customTokens } from '../../../styles';
import { useLanguageContext } from "../../../language/languageContext";
import { AuthHelper } from '../../../libs/auth/AuthHelper';
import { useMsal } from '@azure/msal-react';

const useClasses = makeStyles({
    root: {
        display: 'flex',
        ...shorthands.gap(customTokens.spacingVerticalS),
        flexDirection: 'column',
    },
    card: {
        display: 'flex',
        width: '100%',
        height: 'fit-content',
    }
});

interface ICitationCardsProps {
    message: IChatMessage;
}

export const CitationCards: React.FC<ICitationCardsProps> = ({ message }) => {
    const classes = useClasses();
    const { t } = useLanguageContext();
    const { instance, inProgress } = useMsal();

    let BackendServiceUrl =
    process.env.REACT_APP_BACKEND_URI == null || process.env.REACT_APP_BACKEND_URI.trim() === ''
        ? window.origin
        : process.env.REACT_APP_BACKEND_URI;
    if (!BackendServiceUrl.endsWith('/')) BackendServiceUrl += '/';

    if (!message.citations || message.citations.length === 0) {
        return null;
    }

    const openDocument = async (url: string) => {        
        const accessToken = await AuthHelper.getSKaaSAccessToken(instance, inProgress)
        const response = await fetch(url, {
            method: "GET",
            headers: new Headers({  Authorization: `Bearer ${accessToken}` })
        });
        const newWindow = window.open("", "_blank");
        if (newWindow != null && response.ok) {
            const data = await response.blob();
            const objectURL = URL.createObjectURL(data);
            newWindow.document.write('<iframe width="100%" height="100%" src="' + objectURL + '"></iframe>');
        } else {
            console.error("HTTP-Error: " + response.status);
        }
    };

    return (
        <div className={classes.root}>
            {message.citations.map((citation, index) => {
                return (
                    <Card className={classes.card} size="small" key={`citation-card-${index}`}>
                        <CardHeader
                            image={
                                <Badge shape="rounded" appearance="outline" color="informative">
                                    {index + 1}
                                </Badge>
                            }
                            header={<a style={{ textDecoration: 'underline blue', cursor: 'pointer' }} onClick={() => { openDocument(`${BackendServiceUrl}documents/${citation.link}`).catch(() => { console.error("Error opening window") }) }}>{citation.link}</a>}
                            description={<Caption1>{t("RelevanceScore")}: {citation.relevanceScore.toFixed(3)}</Caption1>}
                        />
                    </Card>
                );
            })}
        </div>
    );
};
